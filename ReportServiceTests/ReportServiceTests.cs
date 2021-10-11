using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using Moq;
using RpiProbeLogger.Communication.Models;
using RpiProbeLogger.Led.Services;
using RpiProbeLogger.Reports.Models;
using RpiProbeLogger.Reports.Services;
using RpiProbeLogger.Sensors.Models;
using System;
using Xunit;

namespace ReportServiceTests
{
    public class ReportServiceTests
    {
        private readonly IFixture _fixture;
        private readonly ReportService _reportService;
        private readonly Mock<IStatusReportService> _statusReportServiceMock;
        private readonly Mock<IReportFileHandler> _reportFileHandlerMock;

        public ReportServiceTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _statusReportServiceMock = _fixture.Freeze<Mock<IStatusReportService>>();
            _reportFileHandlerMock = _fixture.Freeze<Mock<IReportFileHandler>>();
            _reportService = _fixture.Create<ReportService>();
        }

        [Theory]
        [AutoData]
        public void ShouldCreateReportRow_GpsDataIsNull(SenseResponse senseResponse, double? outsideTemperature) =>
            Assert.True(_reportService.WriteReport(senseResponse, null, outsideTemperature));

        [Theory]
        [AutoData]
        public void ShouldCreateReportRow_SenseDataIsNull(GpsModuleResponse gpsModuleResponse, double? outsideTemperature) =>
            Assert.True(_reportService.WriteReport(null, gpsModuleResponse, outsideTemperature));

        [Theory]
        [AutoData]
        public void ShouldCreateReportRow_TemperDataIsNull(SenseResponse senseResponse, GpsModuleResponse gpsModuleResponse) =>
            Assert.True(_reportService.WriteReport(senseResponse, gpsModuleResponse, null));

        [Theory]
        [AutoData]
        public void ShouldCreateReportRow_AllDataNotNull(SenseResponse senseResponse, GpsModuleResponse gpsModuleResponse, double? outsideTemperature) =>
            Assert.True(_reportService.WriteReport(senseResponse, gpsModuleResponse, outsideTemperature));

        [Fact]
        public void ShouldCreateReportRow_AllDataNull() =>
            Assert.True(_reportService.WriteReport(null, null, null));

        [Theory]
        [AutoData]
        public void ShouldShowErrorStatus_IfException(GpsModuleResponse gpsModuleResponse) 
        {
            _reportFileHandlerMock.Setup(handler => handler.WriteRecord(It.IsAny<ReportModel>())).Throws<Exception>();
            _reportService.WriteReport(null, gpsModuleResponse, null);
            _statusReportServiceMock.Verify(r => r.DisplayStatus(It.Is<ReportModel>(r => !r.Status)), Times.Once);
        }

        [Theory]
        [AutoData]
        public void ShouldShowSuccessStatus_IfNoException(GpsModuleResponse gpsModuleResponse)
        {
            _reportService.WriteReport(null, gpsModuleResponse, null);
            _statusReportServiceMock.Verify(r => r.DisplayStatus(It.Is<ReportModel>(r => r.Status)), Times.Once);
        }
    }
}
