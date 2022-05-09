using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RpiProbeLogger.BusModels;
using RpiProbeLogger.TerminalGui.Helpers;
using RpiProbeLogger.TerminalGui.Settings;
using Terminal.Gui;

namespace RpiProbeLogger.TerminalGui
{
    class Program
    {
        private static IHost _host;
        private static IConfiguration Configuration;

        static void Main(string[] args)
        {
            _host = new HostBuilder()
                .ConfigureAppConfiguration((context, builder) => {
                    builder.AddJsonFile("settings.json", true);
                    builder.AddEnvironmentVariables();
                    Configuration = builder.Build();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<TelemetryReceiverHostedService>();
                    services.AddHostedService<LogsReceiverHostedService>();
                    services.AddSingleton<IDirector<Telemetry>, TelemetryDirector>();
                    services.AddSingleton<IDirector<LogEntry>, LogDirector>();
                    services.AddSingleton<View>(provider => provider.GetService<TelemetryView>());
                    services.AddSingleton<TelemetryView>();
                    services.AddSingleton<View>(provider => provider.GetService<LogView>());
                    services.AddSingleton<LogView>();
                    services.Configure<TelemetryReceiverSettings>(Configuration.GetSection("TelemetryReceiverSettings"));
                    services.Configure<LogsReceiverSettings>(Configuration.GetSection("LogsReceiverSettings"));
                    services.AddSingleton<MainWindow>();
                })
                .ConfigureLogging(logConfig =>
                {
                    logConfig.AddConsole();
                })
                .Build();

            var telemetryDirector = _host.Services.GetRequiredService<IDirector<Telemetry>>();
            var logDirector = _host.Services.GetRequiredService<IDirector<LogEntry>>();

            telemetryDirector.OnRefresh += UiRefresh;
            logDirector.OnRefresh += UiRefresh;

            Application.Init();
            Application.Run(_host.Services.GetRequiredService<MainWindow>());
        }

        private static void UiRefresh(object? sender, EventArgs e) => Application.DoEvents();
    }
}