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
using RpiProbeLogger.Extensions;
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
                    services.AddSerialPort("/dev/ttyS0", 115200);
                    services.AddTransient<GpsModuleStatusCommand>();
                    services.AddTransient<GpsModuleCoordinatesCommand>();
                    services.AddSensorDataServices();
                    services.AddTransient<SenseService>();
                    services.AddTransient<IReportService, ReportService>();
                    services.AddSingleton<IStatusReportService, StatusReportService>();
                    services.AddTemper();
                    services.AddTransient<IReportFileHandler, ReportCsvHandler>();
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
