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

        public RpiProbeHostedService(
            GpsModuleStatusCommand gpsModuleStatusCommand,
            GpsModuleCoordinatesCommand gpsModuleCoordinatesCommand,
            SerialPort serialPort,
            ILogger<RpiProbeHostedService> logger,
            SenseService senseService,
            ReportService reportService)
        {
            _gpsModuleStatusCommand = gpsModuleStatusCommand;
            _gpsModuleCoordinatesCommand = gpsModuleCoordinatesCommand;
            _serialPort = serialPort;
            _logger = logger;
            _senseService = senseService;
            _reportService = reportService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //while (true)
            //{
            //    if (Debugger.IsAttached) break;
            //}
            var gpsStatus = _gpsModuleStatusCommand.GetStatus();
            
            if (gpsStatus?.Enabled == false)
                _gpsModuleStatusCommand.SetStatus(
                        new GpsModuleStatusResponse
                        {
                            Enabled = true,
                            Mode = GpsModuleModes.Standalone
                        });
            //var gpsStatusSet = Retry.Do(() =>
            //{
            //    return _gpsModuleStatusCommand.SetStatus(
            //            new GpsModuleStatusResponse
            //            {
            //                Enabled = true,
            //                Mode = GpsModuleModes.Standalone
            //            });
            //}, TimeSpan.FromSeconds(1), (result) => result);

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    return Task.CompletedTask;

                var gpsData = _gpsModuleCoordinatesCommand.GetGpsData();
                if (gpsData != null)
                {
                    var senseData = _senseService.GetSensorsData();
                    try
                    {
                        _reportService.WriteReport(senseData, gpsData, 0);
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
