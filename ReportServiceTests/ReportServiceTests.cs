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
        private readonly IReportService _reportService;
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
        public void ShouldCreateReportRow_GpsDataIsNull(SenseResponse senseResponse, OutsideTemperatureResponse outsideTemperatureResponse) =>
            Assert.True(_reportService.WriteReport(senseResponse, default, outsideTemperatureResponse));

        [Theory]
        [AutoData]
        public void ShouldCreateReportRow_SenseDataIsNull(GpsModuleResponse gpsModuleResponse, OutsideTemperatureResponse outsideTemperatureResponse) =>
            Assert.True(_reportService.WriteReport(default, gpsModuleResponse, outsideTemperatureResponse));

        [Theory]
        [AutoData]
        public void ShouldCreateReportRow_TemperDataIsNull(SenseResponse senseResponse, GpsModuleResponse gpsModuleResponse) =>
            Assert.True(_reportService.WriteReport(senseResponse, gpsModuleResponse, default));

        [Theory]
        [AutoData]
        public void ShouldCreateReportRow_AllDataNotNull(SenseResponse senseResponse, GpsModuleResponse gpsModuleResponse, OutsideTemperatureResponse outsideTemperatureResponse) =>
            Assert.True(_reportService.WriteReport(senseResponse, gpsModuleResponse, outsideTemperatureResponse));

        [Fact]
        public void ShouldCreateReportRow_AllDataNull() =>
            Assert.True(_reportService.WriteReport(default, default, default));

        [Theory]
        [AutoData]
        public void ShouldShowErrorStatus_IfException(GpsModuleResponse gpsModuleResponse)
        {
            _reportFileHandlerMock.Setup(handler => handler.WriteRecord(It.IsAny<ReportModel>())).Throws<Exception>();
            _reportService.WriteReport(default, gpsModuleResponse, default);
            _statusReportServiceMock.Verify(r => r.DisplayStatus(It.Is<ReportModel>(r => !r.Status)), Times.Once);
        }

        [Theory]
        [AutoData]
        public void ShouldShowSuccessStatus_IfNoException(GpsModuleResponse gpsModuleResponse)
        {
            _reportService.WriteReport(default, gpsModuleResponse, default);
            _statusReportServiceMock.Verify(r => r.DisplayStatus(It.Is<ReportModel>(r => r.Status)), Times.Once);
        }
    }
}
