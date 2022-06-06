using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetMQ.Sockets;
using RpiProbeLogger.Bus;
using RpiProbeLogger.Bus.Telemetry;
using RpiProbeLogger.Communication.Settings;
using RpiProbeLogger.Led.Services;
using RpiProbeLogger.Reports.Services;
using RpiProbeLogger.Sensors.Services;
using Sense.RTIMU;
using System;
using System.IO.Ports;

namespace RpiProbeLogger.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSerialPort(this IServiceCollection services, string portName = null, int? baudRate = null)
        {
            var serialPort = new SerialPort(portName ?? "/dev/ttyS0", baudRate ?? 115200)
            {
                ReadTimeout = 500,
                WriteTimeout = 500,
                NewLine = "\r"
            };
            serialPort.Open();
            services.AddSingleton(serialPort);
            return services;
        }

        public static IServiceCollection AddSerialPort(this IServiceCollection services, IConfiguration configuration)
        {
            var portSettings = configuration.GetSection("SerialPortOptions").Get<SerialPortOptions>();
            var serialPort = new SerialPort(portSettings.PortName, portSettings.BaudRate)
            {
                ReadTimeout = portSettings.ReadTimeout,
                WriteTimeout = portSettings.WriteTimeout,
                NewLine = portSettings.NewLine
            };
            serialPort.Open();
            services.AddSingleton(serialPort);
            return services;
        }

        public static IServiceCollection AddSensorDataServices(this IServiceCollection services)
        {
            services.AddSingleton<RTIMUSettings>((_) => RTIMUSettings.CreateDefault());
            services.AddSingleton<RTIMU>((provider) =>
            {
                var muSettings = provider.GetService<RTIMUSettings>();
                return muSettings.CreateIMU();
            });
            services.AddSingleton<RTPressure>((provider) =>
            {
                var muSettings = provider.GetService<RTIMUSettings>();
                return muSettings.CreatePressure();
            });
            services.AddSingleton<RTHumidity>((provider) =>
            {
                var muSettings = provider.GetService<RTIMUSettings>();
                return muSettings.CreateHumidity();
            });

            services.AddTransient<ISenseIMUService, SenseIMUService>();
            services.AddTransient<ISenseHumidityService, SenseHumidityService>();
            services.AddTransient<ISensePressureService, SensePressureService>();

            services.AddTransient<ISenseService, SenseService>();
            return services;
        }

        public static IServiceCollection AddTemper(this IServiceCollection services, int? productId = null, int? vendorId = null)
        {
            services.AddSingleton<IHidDeviceHandler, TemperDeviceHandler>((provider) =>
            {
                var statusReportService = provider.GetRequiredService<IStatusReportService>();
                var deviceHandler = new TemperDeviceHandler(productId ?? 0x2107, vendorId ?? 0x413D, statusReportService);
                deviceHandler.OpenDevice();
                return deviceHandler;
            });
            services.AddSingleton<ITemperService, TemperService>();
            return services;
        }

        public static IServiceCollection AddReportingServices(this IServiceCollection services, Action<TelemetryReporterOptions> busOptions)
        {
            AddReportingServices(services);
            services.Configure(busOptions);
            return services;
        }

        public static IServiceCollection AddReportingServices(this IServiceCollection services, IConfiguration configuration)
        {
            AddReportingServices(services);
            services.Configure<TelemetryReporterOptions>(configuration.GetSection("TelemetryReporterOptions"));
            return services;
        }

        private static void AddReportingServices(IServiceCollection services)
        {
            services.TryAddTransient<PublisherSocket>();
            services.TryAddTransient<IBusReporter, BusReporter>();
            services.AddTransient<ILedMatrix, LedMatrix>();
            services.AddTransient<IReportService, ReportService>()
                .Decorate<IReportService, TelemetryReporter>();
            services.AddSingleton<IStatusReportService, StatusReportService>();
            services.AddTransient<IReportFileHandler, ReportCsvHandler>();
        }
    }
}
