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
        private readonly TabView _tabView = new()
        {
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            X = 0,
            Y = 0
        };

        public MainWindow(IHost host, IEnumerable<View> views) : base()
        {
            _host = host;

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;

            foreach (var view in views)
                _tabView.AddTab(new TabView.Tab(view.Id?.ToString(), view), false);

            _tabView.SelectedTab = _tabView.Tabs.First();
            Add(_tabView);
        }

        private void MainWindow_Closing(ToplevelClosingEventArgs obj) =>
            _host
                .StopAsync()
                .GetAwaiter()
                .GetResult();

        private void MainWindow_Loaded() => Task.Run(() => _host.RunAsync());
    }
}
