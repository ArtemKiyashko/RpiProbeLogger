using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace RpiProbeLogger.TerminalGui
{
    public class TelemetryWindow : Window
    {
        public TelemetryWindow() : base("Telemetry")
        {
            Width = Dim.Fill();
            Height = Dim.Fill();
            X = 0;
            Y = 0;
            ColorScheme = Colors.TopLevel;
        }
    }
}
