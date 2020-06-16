using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RpiProbeLogger.Communication.Commands;
using RpiProbeLogger.Communication.Models;
using RpiProbeLogger.Helpers;
using RpiProbeLogger.Reports.Services;
using RpiProbeLogger.Sensors.Services;
using Sense.RTIMU;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
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
        private readonly SenseService _senseService;
        private readonly ReportService _reportService;
        private readonly TemperService _temperService;

        public RpiProbeHostedService(
            GpsModuleStatusCommand gpsModuleStatusCommand,
            GpsModuleCoordinatesCommand gpsModuleCoordinatesCommand,
            SerialPort serialPort,
            ILogger<RpiProbeHostedService> logger,
            SenseService senseService,
            ReportService reportService,
            TemperService temperService)
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
                if (gpsData != null || _reportService.ReportFileCreated)
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
