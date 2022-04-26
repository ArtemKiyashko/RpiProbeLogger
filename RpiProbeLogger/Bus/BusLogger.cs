using Microsoft.Extensions.Logging;
using RpiProbeLogger.BusModels;
using System;

namespace RpiProbeLogger.Bus
{
    public class BusLogger : ILogger
    {
        private readonly IBusReporter _busReporter;

        public BusLogger(IBusReporter busReporter)
        {
            _busReporter = busReporter;
        }

        public IDisposable BeginScope<TState>(TState state) => default;

        public bool IsEnabled(LogLevel logLevel) => logLevel == LogLevel.Error;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;
            _busReporter.Send($"" +
                    $"{formatter(state, exception)}" +
                    $"{Environment.NewLine}" +
                    $"{exception.Message}" +
                    $"{Environment.NewLine}" +
                    $"{exception.StackTrace}", 
                Constants.ERROR_TOPIC_NAME).GetAwaiter().GetResult();
        }
    }
}
