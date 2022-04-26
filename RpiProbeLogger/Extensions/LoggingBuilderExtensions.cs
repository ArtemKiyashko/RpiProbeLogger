using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RpiProbeLogger.Bus;

namespace RpiProbeLogger.Extensions
{
    public static class LoggingBuilderExtensions
    {
        public static ILoggingBuilder AddZeromMqLogger(this ILoggingBuilder builder, string bindAddress)
        {
            builder.Services.TryAddSingleton<IBusReporter>((provider) => new BusReporter(bindAddress));
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, BusLoggerProvider>());
            return builder;
        }
    }
}
