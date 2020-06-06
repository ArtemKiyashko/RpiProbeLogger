using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RpiProbeLogger.Communication.Commands;
using RpiProbeLogger.Communication.Models;
using RpiProbeLogger.Helpers;
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

        public RpiProbeHostedService(
            GpsModuleStatusCommand gpsModuleStatusCommand,
            GpsModuleCoordinatesCommand gpsModuleCoordinatesCommand,
            SerialPort serialPort,
            ILogger<RpiProbeHostedService> logger)
        {
            _gpsModuleStatusCommand = gpsModuleStatusCommand;
            _gpsModuleCoordinatesCommand = gpsModuleCoordinatesCommand;
            _serialPort = serialPort;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            for (; ; )
            {
                Console.WriteLine("waiting for debugger attach");
                if (Debugger.IsAttached) break;
                Thread.Sleep(1000);
            }
            var gpsStatusSet = Retry.Do(() =>
            {
                return _gpsModuleStatusCommand.SetStatus(
                        new GpsModuleStatusResponse
                        {
                            Enabled = true,
                            Mode = GpsModuleModes.Standalone
                        });
            }, TimeSpan.FromSeconds(1), (result) => result);

            if (!gpsStatusSet)
                throw new Exception("Cannot enable GPS Module");

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    return Task.CompletedTask;

                var gpsData = _gpsModuleCoordinatesCommand.GetGpsData();
                Thread.Sleep(1000);
            }
        }

        private void _gpsModuleCoordinatesCommand_OnCoordinatesReceived(GpsModuleResponse gpsModuleResponse)
        {
            var test = gpsModuleResponse;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _serialPort.Close();
            return Task.CompletedTask;
        }
    }
}
