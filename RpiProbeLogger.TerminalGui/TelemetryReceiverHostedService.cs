using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NetMQ;
using NetMQ.Sockets;
using RpiProbeLogger.BusModels;
using RpiProbeLogger.TerminalGui.Helpers;
using RpiProbeLogger.TerminalGui.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RpiProbeLogger.TerminalGui
{
    public class TelemetryReceiverHostedService : IHostedService
    {
        private readonly IDirector<Telemetry> _telemetryDirector;
        private TelemetryReceiverSettings _settings;
        private readonly SubscriberSocket _subscriber = new();
        private string _fullAddress => $"tcp://{_settings.Ip}:{_settings.Port}";

        public TelemetryReceiverHostedService(
            IDirector<Telemetry> telemetryDirector, 
            IOptions<TelemetryReceiverSettings> options,
            TelemetryView telemetryWindow)
        {
            _telemetryDirector = telemetryDirector;
            _settings = options.Value;
            _telemetryDirector.Setup(telemetryWindow);
            _telemetryDirector.Refresh(default);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.Connect(_fullAddress);
            _subscriber.SubscribeToAnyTopic();

            while (true)
            {
                var msg = _subscriber.ReceiveFrameString();
                Telemetry model = JsonSerializer.Deserialize<Telemetry>(msg);
                _telemetryDirector.Refresh(model);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _subscriber.Disconnect(_fullAddress);
            _subscriber.Dispose();
            return Task.CompletedTask;
        }
    }
}
