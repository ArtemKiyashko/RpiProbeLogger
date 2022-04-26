using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpiProbeLogger.Bus
{
    [ProviderAlias("ZeroMq")]
    public sealed class BusLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, BusLogger> _loggers =
            new(StringComparer.OrdinalIgnoreCase);
        private readonly IBusReporter _busReporter;

        public BusLoggerProvider(IBusReporter busReporter) =>
            _busReporter = busReporter;

        public ILogger CreateLogger(string categoryName) =>
            _loggers.GetOrAdd(categoryName, name => new BusLogger(_busReporter));

        public void Dispose() =>
            _loggers.Clear();
    }
}
