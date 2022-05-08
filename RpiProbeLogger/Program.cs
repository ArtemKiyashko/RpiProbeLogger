using Microsoft.Extensions.Configuration;
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
        private static IConfiguration Configuration;
        static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureAppConfiguration((hostContext, builder) =>
                {
                    builder.AddJsonFile("settings.json");
                    builder.AddEnvironmentVariables();
                    Configuration = builder.Build();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<RpiProbeHostedService>();
                    services.AddSerialPort("/dev/ttyS0", 115200);
                    services.AddTransient<GpsModuleStatusCommand>();
                    services.AddTransient<GpsModuleCoordinatesCommand>();
                    services.AddSensorDataServices();
                    services.AddTemper();
                    services.AddReportingServices(Configuration);
                })
                .ConfigureLogging(logConfig =>
                {
                    logConfig.SetMinimumLevel(LogLevel.Information);
                    logConfig.AddConsole();
                    logConfig.AddZeromMqLogger(Configuration);
                })
                .Build();
            await host.RunAsync();
        }
    }
}
