using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RpiProbeLogger.Communication.Commands;
using RpiProbeLogger.Led.Services;
using RpiProbeLogger.Reports.Services;
using RpiProbeLogger.Sensors.Services;
using Sense.RTIMU;

namespace RpiProbeLogger
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureServices((hostContext, services) => {
                    services.AddHostedService<RpiProbeHostedService>();
                    services.AddSingleton<SerialPort>((_) => { 
                        var serialPort = new SerialPort("/dev/ttyS0", 115200);
                        serialPort.ReadTimeout = 500;
                        serialPort.WriteTimeout = 500;
                        serialPort.NewLine = "\r";
                        serialPort.Open();
                        return serialPort;
                    });
                    services.AddTransient<GpsModuleStatusCommand>();
                    services.AddTransient<GpsModuleCoordinatesCommand>();
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
                    services.AddTransient<SenseService>();
                    services.AddTransient<ReportService>();
                    services.AddSingleton<StatusReportService>();
                    services.AddSingleton<TemperService>();
                })
                .ConfigureLogging(logConfig =>
                {
                    logConfig.SetMinimumLevel(LogLevel.Information);
                    logConfig.AddConsole();
                })
                .Build();
            await host.RunAsync();
        }
    }
}
