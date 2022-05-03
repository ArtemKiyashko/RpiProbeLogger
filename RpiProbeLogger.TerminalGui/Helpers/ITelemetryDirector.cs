using RpiProbeLogger.BusModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace RpiProbeLogger.TerminalGui.Helpers
{
    public interface ITelemetryDirector
    {
        public void Setup(Window container);
        public void Refresh(Telemetry telemetry);
    }
}
