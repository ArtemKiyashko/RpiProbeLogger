using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RpiProbeLogger.BusModels;
using RpiProbeLogger.TerminalGui.Helpers;
using RpiProbeLogger.TerminalGui.Settings;

namespace RpiProbeLogger.TerminalGui
{
    public class LogsReceiverHostedService : BaseReceiver<LogEntry>, IHostedService
    {
        private LogsReceiverSettings _settings;
        private string _fullAddress => $"tcp://{_settings.Ip}:{_settings.Port}";

        public LogsReceiverHostedService(
            IDirector<LogEntry> logsDirector, 
            IOptions<LogsReceiverSettings> options,
            LogView logsWindow) : base(logsDirector)
        {
            _settings = options.Value;
            _director.Setup(logsWindow);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.Connect(_fullAddress);
            _subscriber.SubscribeToAnyTopic();

            Task.Run(() => ReceiverLoop(), cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _subscriber.Disconnect(_fullAddress);
            _subscriber.Dispose();
            return Task.CompletedTask;
        }
    }
}
