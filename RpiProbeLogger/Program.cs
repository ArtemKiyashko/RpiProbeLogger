using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RpiProbeLogger.Communication.Commands;
using RpiProbeLogger.Extensions;
using RpiProbeLogger.Led.Services;
using RpiProbeLogger.Reports.Services;
using System.Threading.Tasks;
using RpiProbeLogger.BusModels;

namespace RpiProbeLogger
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<TestHostedService>();
                    //services.AddHostedService<RpiProbeHostedService>();
                    //services.AddSerialPort("/dev/ttyS0", 115200);
                    //services.AddTransient<GpsModuleStatusCommand>();
                    //services.AddTransient<GpsModuleCoordinatesCommand>();
                    //services.AddSensorDataServices();
                    //services.AddTransient<IReportService, ReportService>();
                    //services.AddSingleton<IStatusReportService, StatusReportService>();
                    //services.AddTemper();
                    //services.AddTransient<IReportFileHandler, ReportCsvHandler>();
                })
                .ConfigureLogging(logConfig =>
                {
                    logConfig.SetMinimumLevel(LogLevel.Information);
                    logConfig.AddConsole();
                    logConfig.AddZeromMqLogger(Constants.BIND_ADDRESS);
                })
                .Build();
            await host.RunAsync();
        }
    }
}
