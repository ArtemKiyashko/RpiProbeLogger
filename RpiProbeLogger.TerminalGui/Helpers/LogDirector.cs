using RpiProbeLogger.BusModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace RpiProbeLogger.TerminalGui.Helpers
{
    public class LogDirector : IDirector<LogEntry>
    {
        private LogViewBuilder _viewBuilder;
        private Label _view;

        public void Refresh(LogEntry telemetry)
        {

        }

        public void Setup(View container)
        {
            _viewBuilder = new LogViewBuilder(container);
            _view = _viewBuilder.Build();
        }
    }
}
