using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using RpiProbeLogger.Communication.Models;
using RpiProbeLogger.Reports.Models;
using RpiProbeLogger.Reports.Services;
using System;
using Xunit;

namespace ReportCsvHandlerTests
{
    public class ReportCsvHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly IReportFileHandler _reportCsvHandler;

        public ReportCsvHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _reportCsvHandler = _fixture.Create<ReportCsvHandler>();
        }

        [Theory]
        [AutoData]
        public void ShoudWriteSuccessfullyNextRow_IfNoOutsideTemperatureWrittenBefore(ReportModel reportModel)
        {
            GpsModuleResponse gpsModuleResponse = new() { DateTimeUtc = new DateTime(1986, 1, 1) };
            _reportCsvHandler.CreateFile<ReportModel>(gpsModuleResponse);
            _reportCsvHandler.WriteRecord(reportModel);
        }
    }
}