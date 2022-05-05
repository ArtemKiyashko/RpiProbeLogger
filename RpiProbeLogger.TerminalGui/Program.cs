using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RpiProbeLogger.TerminalGui.Helpers;
using RpiProbeLogger.TerminalGui.Settings;
using Terminal.Gui;

namespace RpiProbeLogger.TerminalGui
{
    class Program
    {
        private static IHost _host;

        static void Main(string[] args)
        {
            _host = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<TelemetryReceiverHostedService>();
                    services.AddSingleton<ITelemetryDirector, TelemetryDirector>();
                    services.AddSingleton<Window>(provider => provider.GetService<TelemetryWindow>());
                    services.AddSingleton<TelemetryWindow>();
                    services.Configure<TelemetryReceiverSettings>(options =>
                    {
                        options.Ip = "127.0.0.1";
                        options.Port = 5557;
                    });
                })
                .Build();

            Application.Init();
            Application.MainLoop.AddTimeout(TimeSpan.FromMilliseconds(200), (loop) => AppTick());
            Application.Run(new MainWindow(_host, _host.Services.GetServices<Window>().ToArray()));
        }

        static bool AppTick()
        {
            Application.MainLoop.MainIteration();
            return true;
        }
    }
}