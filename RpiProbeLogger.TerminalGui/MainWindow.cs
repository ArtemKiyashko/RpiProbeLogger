using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RpiProbeLogger.TerminalGui.Helpers;
using RpiProbeLogger.TerminalGui.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace RpiProbeLogger.TerminalGui
{
    public class MainWindow : Toplevel
    {
        private readonly IHost _host;

        public MainWindow(IHost host, params Window[] windows) : base()
        {
            _host = host;

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;

            Add(windows);
        }

        private void MainWindow_Closing(ToplevelClosingEventArgs obj) =>
            _host
                .StopAsync()
                .GetAwaiter()
                .GetResult();

        private void MainWindow_Loaded() => Task.Run(() => _host.Run());
    }
}
