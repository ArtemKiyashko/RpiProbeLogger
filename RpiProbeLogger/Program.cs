using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RpiProbeLogger.Communication.Commands;
using RpiProbeLogger.Extensions;
using System.Threading.Tasks;

namespace RpiProbeLogger
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<RpiProbeHostedService>();
                    services.AddSerialPort("/dev/ttyS0", 115200);
                    services.AddTransient<GpsModuleStatusCommand>();
                    services.AddTransient<GpsModuleCoordinatesCommand>();
                    services.AddSensorDataServices();
                    services.AddTemper();
                    services.AddReportingServices(options => options.Port = 5557);
                })
                .ConfigureLogging(logConfig =>
                {
                    logConfig.SetMinimumLevel(LogLevel.Information);
                    logConfig.AddConsole();
                    logConfig.AddZeromMqLogger(options => options.Port = 5556);
                })
                .Build();
            await host.RunAsync();
        }
    }
}
