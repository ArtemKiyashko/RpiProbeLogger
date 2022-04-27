using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpiProbeLogger.Bus.Logger
{
    [ProviderAlias("ZeroMq")]
    public sealed class BusLoggerProvider : ILoggerProvider
    {
        private readonly IDisposable _onChangeToken;
        private BusLogger _logger;
        private readonly IBusReporter _busReporter;
        private BusLoggerOptions _busLoggerOptions;

        public BusLoggerProvider(IBusReporter busReporter, IOptionsMonitor<BusLoggerOptions> optionsMonitor)
        {
            _busReporter = busReporter;
            _busLoggerOptions = optionsMonitor.CurrentValue;
            _onChangeToken = optionsMonitor.OnChange(updateConfig => _busLoggerOptions = updateConfig);
            _busReporter.BindPort(_busLoggerOptions.Port);
        }

        public ILogger CreateLogger(string categoryName)
        {
            if (_logger is not null) return _logger;
            _logger = new BusLogger(_busReporter);
            return _logger;
        }

        public void Dispose()
        {
            _onChangeToken.Dispose();
            _logger = null;
        }
    }
}
