using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RpiProbeLogger.Bus.Logger;
using RpiProbeLogger.Bus;
using Microsoft.Extensions.Logging.Configuration;
using System;
using NetMQ.Sockets;

namespace RpiProbeLogger.Extensions
{
    public static class LoggingBuilderExtensions
    {
        public static ILoggingBuilder AddZeromMqLogger(this ILoggingBuilder builder)
        {
            builder.AddConfiguration();

            builder.Services.TryAddTransient<PublisherSocket>();
            builder.Services.TryAddTransient<IBusReporter, BusReporter>();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, BusLoggerProvider>());
            LoggerProviderOptions.RegisterProviderOptions<BusLoggerOptions, BusLoggerProvider>(builder.Services);

            return builder;
        }

        public static ILoggingBuilder AddZeromMqLogger(this ILoggingBuilder builder, Action<BusLoggerOptions> config)
        {
            builder.AddZeromMqLogger();
            builder.Services.Configure(config);
            return builder;
        }
    }
}
