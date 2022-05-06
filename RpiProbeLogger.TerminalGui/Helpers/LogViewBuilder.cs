using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace RpiProbeLogger.TerminalGui.Helpers
{
    public class LogViewBuilder
    {
        private readonly View _container;

        public LogViewBuilder(View container) =>
            _container = container;

        public Label Build() => new();
    }
}
