using Microsoft.Extensions.Options;
using RpiProbeLogger.Communication.Models;
using RpiProbeLogger.Reports.Models;
using RpiProbeLogger.Reports.Services;
using RpiProbeLogger.Sensors.Models;
using System;
using System.Threading.Tasks;

namespace RpiProbeLogger.Bus.Telemetry
{
    public class TelemetryReporter : IReportService, IDisposable
    {
        private readonly IDisposable _onChangeToken;
        private readonly IBusReporter _busReporter;
        private readonly IReportService _decoratee;
        private TelemetryReporterOptions _options;

        public TelemetryReporter(
            IBusReporter busReporter, 
            IOptionsMonitor<TelemetryReporterOptions> optionsMonitor, 
            IReportService decoratee)
        {
            _busReporter = busReporter;
            _decoratee = decoratee;
            _options = optionsMonitor.CurrentValue;
            _busReporter.BindPort(_options.Port);
            _onChangeToken = optionsMonitor.OnChange(updateOptions => _options = updateOptions);
        }

        public bool ReportFileCreated => _decoratee.ReportFileCreated;

        public void Dispose() => _onChangeToken.Dispose();

        public async Task<ReportModel> WriteReport(SenseResponse senseResponse, GpsModuleResponse gpsModuleResponse, OutsideTemperatureResponse outsideTemperatureResponse)
        {
            var result = await _decoratee.WriteReport(senseResponse, gpsModuleResponse, outsideTemperatureResponse);
            await _busReporter.Send(MapToTelemetry(result));
            return result;
        }

        private static BusModels.Telemetry MapToTelemetry(ReportModel reportModel) => new(
                reportModel.Latitude,
                reportModel.Longitude,
                reportModel.DateTimeUtc,
                reportModel.Altitude,
                reportModel.Speed,
                reportModel.Course,
                reportModel.FusionPose,
                reportModel.FusionQPose,
                reportModel.Gyro,
                reportModel.Accel,
                reportModel.Compass,
                reportModel.Pressure,
                reportModel.PressureTemperature,
                reportModel.Humidity,
                reportModel.HumidityTemperature,
                reportModel.OutsideTemperature
            );
    }
}
