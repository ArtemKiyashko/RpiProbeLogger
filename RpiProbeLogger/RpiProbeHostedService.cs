using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RpiProbeLogger.Communication.Commands;
using RpiProbeLogger.Communication.Models;
using RpiProbeLogger.Reports.Services;
using RpiProbeLogger.Sensors.Services;
using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;


namespace RpiProbeLogger
{
    public class RpiProbeHostedService : IHostedService
    {
        private readonly GpsModuleStatusCommand _gpsModuleStatusCommand;
        private readonly GpsModuleCoordinatesCommand _gpsModuleCoordinatesCommand;
        private readonly SerialPort _serialPort;
        private readonly ILogger<RpiProbeHostedService> _logger;
        private readonly ISenseService _senseService;
        private readonly IReportService _reportService;
        private readonly ITemperService _temperService;

        public RpiProbeHostedService(
            GpsModuleStatusCommand gpsModuleStatusCommand,
            GpsModuleCoordinatesCommand gpsModuleCoordinatesCommand,
            SerialPort serialPort,
            ILogger<RpiProbeHostedService> logger,
            ISenseService senseService,
            IReportService reportService,
            ITemperService temperService)
        {
            _gpsModuleStatusCommand = gpsModuleStatusCommand;
            _gpsModuleCoordinatesCommand = gpsModuleCoordinatesCommand;
            _serialPort = serialPort;
            _logger = logger;
            _senseService = senseService;
            _reportService = reportService;
            _temperService = temperService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var gpsStatus = _gpsModuleStatusCommand.GetStatus();

            if (gpsStatus?.Enabled == false)
                _gpsModuleStatusCommand.SetStatus(
                        new GpsModuleStatusResponse
                        {
                            Enabled = true,
                            Mode = GpsModuleModes.Standalone
                        });

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    return Task.CompletedTask;

                var gpsData = _gpsModuleCoordinatesCommand.GetGpsData();
                if (gpsData is not null || _reportService.ReportFileCreated)
                {
                    var senseData = _senseService.GetSensorsData();
                    var outsideTemperatureResponse = _temperService.ReadTemperature();
                    try
                    {
                        _reportService.WriteReport(senseData, gpsData, outsideTemperatureResponse?.OutsideTemperature);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error writing report");
                    }
                }
                Thread.Sleep(1000);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _serialPort.Close();
            return Task.CompletedTask;
        }
    }
}
