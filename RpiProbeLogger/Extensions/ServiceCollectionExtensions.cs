using Microsoft.Extensions.DependencyInjection;
using RpiProbeLogger.Helpers;
using RpiProbeLogger.Led.Services;
using RpiProbeLogger.Sensors.Services;
using Sense.RTIMU;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

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

        public static IServiceCollection AddSensorDataServices(this IServiceCollection services)
        {
            services.AddSingleton<RTIMUSettings>((_) => RTIMUSettings.CreateDefault());
            services.AddSingleton<RTIMU>((provider) => {
                var muSettings = provider.GetService<RTIMUSettings>();
                return muSettings.CreateIMU();
            });
            services.AddSingleton<RTPressure>((provider) => {
                var muSettings = provider.GetService<RTIMUSettings>();
                return muSettings.CreatePressure();
            });
            services.AddSingleton<RTHumidity>((provider) => {
                var muSettings = provider.GetService<RTIMUSettings>();
                return muSettings.CreateHumidity();
            });

            services.AddTransient<ISenseService, SenseService>();
            return services;
        }

        public static IServiceCollection AddTemper(this IServiceCollection services, int? productId = null, int? vendorId = null)
        {
            services.AddSingleton<IHidDeviceHandler, TemperDeviceHandler>((provider) => {
                var statusReportService = provider.GetRequiredService<IStatusReportService>();
                var deviceHandler = new TemperDeviceHandler(productId ?? 0x2107, vendorId ?? 0x413D, statusReportService);
                deviceHandler.OpenDevice();
                return deviceHandler;
            });
            services.AddSingleton<ITemperService, TemperService>();
            return services;
        }
    }
}
