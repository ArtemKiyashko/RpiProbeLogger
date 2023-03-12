using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using CsvHelper;
using FluentAssertions;
using Moq;
using RpiProbeLogger.Communication.Models;
using RpiProbeLogger.Led.Services;
using RpiProbeLogger.Reports.Models;
using RpiProbeLogger.Reports.Services;
using RpiProbeLogger.Sensors.Models;
using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        public async Task ShouldCreateReportRow_GpsDataIsNull_ReportFileCreated(SenseResponse senseResponse, GpsModuleResponse gpsModuleResponse, OutsideTemperatureResponse outsideTemperatureResponse)
        {
            await _reportService.WriteReport(senseResponse, gpsModuleResponse, outsideTemperatureResponse);

            Assert.True((await _reportService.WriteReport(senseResponse, default, outsideTemperatureResponse)).Status);
        }

        [Theory]
        [AutoData]
        public async Task ShouldThrowException_GpsDataIsNull_ReportNotFileCreated(SenseResponse senseResponse, OutsideTemperatureResponse outsideTemperatureResponse) =>
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _reportService.WriteReport(senseResponse, default, outsideTemperatureResponse));

        [Theory]
        [AutoData]
        public async Task ShouldCreateReportRow_SenseDataIsNull(GpsModuleResponse gpsModuleResponse, OutsideTemperatureResponse outsideTemperatureResponse) =>
            Assert.True((await _reportService
                .WriteReport(default, gpsModuleResponse, outsideTemperatureResponse))
                .Status);

        [Theory]
        [AutoData]
        public async Task ShouldCreateReportRow_TemperDataIsNull(SenseResponse senseResponse, GpsModuleResponse gpsModuleResponse) =>
            Assert.True((await _reportService
                .WriteReport(senseResponse, gpsModuleResponse, default))
                .Status);

        [Theory]
        [AutoData]
        public async Task ShouldCreateReportRow_AllDataNotNull(SenseResponse senseResponse, GpsModuleResponse gpsModuleResponse, OutsideTemperatureResponse outsideTemperatureResponse) =>
            Assert.True((await _reportService
                .WriteReport(senseResponse, gpsModuleResponse, outsideTemperatureResponse))
                .Status);

        [Theory]
        [AutoData]
        public async Task ShouldCreateReportRow_AllDataNull_ReportFileCreated(GpsModuleResponse gpsModuleResponse)
        {
            await _reportService.WriteReport(default, gpsModuleResponse, default);

            Assert.True((await _reportService.WriteReport(default, default, default)).Status);
        }

        [Theory]
        [AutoData]
        public async Task ShouldShowErrorStatus_IfException(GpsModuleResponse gpsModuleResponse)
        {
            _reportFileHandlerMock.Setup(handler => handler.WriteRecord(It.IsAny<ReportModel>())).Throws<Exception>();
            await _reportService.WriteReport(default, gpsModuleResponse, default);
            _statusReportServiceMock.Verify(r => r.DisplayStatus(It.Is<ReportModel>(r => !r.Status)), Times.Once);
        }

        [Theory]
        [AutoData]
        public async Task ShouldShowSuccessStatus_IfNoException(GpsModuleResponse gpsModuleResponse)
        {
            await _reportService.WriteReport(default, gpsModuleResponse, default);
            _statusReportServiceMock.Verify(r => r.DisplayStatus(It.Is<ReportModel>(r => r.Status)), Times.Once);
        }

        [Theory]
        [AutoData]
        public async Task ShoudCallCreateFileOne_IfGpsDataLostInFlight(SenseResponse senseResponse, GpsModuleResponse gpsModuleResponse, OutsideTemperatureResponse outsideTemperatureResponse)
        {
            await _reportService
                .WriteReport(senseResponse, gpsModuleResponse, outsideTemperatureResponse); //begin normally with GPS data available

            await _reportService
                .WriteReport(senseResponse, default, outsideTemperatureResponse); //simulate GPS data lost in-flight

            _reportFileHandlerMock.Verify(f => f.CreateFile<ReportModel>(It.IsAny<GpsModuleResponse>()), Times.Once);
        }
    }
}
