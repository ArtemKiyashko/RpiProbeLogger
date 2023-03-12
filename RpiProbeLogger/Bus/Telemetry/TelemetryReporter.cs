using Microsoft.Extensions.Logging;
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
        private readonly ILogger<TelemetryReporter> _logger;
        private TelemetryReporterOptions _options;
        private bool _busPortReady = false;

        public TelemetryReporter(
            IBusReporter busReporter, 
            IOptionsMonitor<TelemetryReporterOptions> optionsMonitor, 
            IReportService decoratee,
            ILogger<TelemetryReporter> logger)
        {
            _busReporter = busReporter;
            _decoratee = decoratee;
            _logger = logger;
            _options = optionsMonitor.CurrentValue;
            _onChangeToken = optionsMonitor.OnChange(updateOptions => _options = updateOptions);
            BusInit();
        }

        protected void BusInit()
        {
            try
            {
                _busReporter.BindPort(_options.Port);
                _busPortReady = true;
            }
            catch (Exception ex)
            {
                _busPortReady = false;
                _logger.LogError(ex, "Cannot bind message bus port for telemetry");
            }
        }

        public void Dispose() => _onChangeToken.Dispose();

        public async Task<ReportModel> WriteReport(SenseResponse senseResponse, GpsModuleResponse gpsModuleResponse, OutsideTemperatureResponse outsideTemperatureResponse)
        {
            var result = await _decoratee.WriteReport(senseResponse, gpsModuleResponse, outsideTemperatureResponse);

            if (!_busPortReady) return result;
            await SendTelemetry(result);
            return result;
        }

        private async Task SendTelemetry(ReportModel result)
        {
            try
            {
                await _busReporter.Send(MapToTelemetry(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot send telemetry to message bus");
            }
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
