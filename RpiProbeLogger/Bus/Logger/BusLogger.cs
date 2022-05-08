using Microsoft.Extensions.Logging;
using RpiProbeLogger.BusModels;
using System;

namespace RpiProbeLogger.Bus.Logger
{
    public class BusLogger : ILogger
    {
        private readonly IBusReporter _busReporter;

        public BusLogger(IBusReporter busReporter) => _busReporter = busReporter;

        public IDisposable BeginScope<TState>(TState state) => default;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;
            try
            {
                _busReporter.Send(
                    new LogEntry(
                        logLevel,
                        formatter(state, exception),
                        exception?.Message,
                        exception?.StackTrace))
                    .GetAwaiter().GetResult();
            }
            catch { }
        }
    }
}
