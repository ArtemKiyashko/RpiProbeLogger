using RpiProbeLogger.BusModels;
using RpiProbeLogger.TerminalGui.Extensions;
using System.Diagnostics.CodeAnalysis;
using Terminal.Gui;

namespace RpiProbeLogger.TerminalGui.Helpers
{
    public class LogViewBuilder
    {
        private readonly View _container;
        private LogLabel _logView;

        public LogViewBuilder([NotNull]View container) => _container = container;

        public LogViewBuilder AddLogView()
        {
            _logView = new LogLabel()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
           
            _container.Add(_logView);
            return this;
        }

        public LogLabel Build() => _logView;

    }
}
