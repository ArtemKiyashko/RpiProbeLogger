using Terminal.Gui;

namespace RpiProbeLogger.TerminalGui
{
    public class TelemetryView : View
    {
        public TelemetryView() : base()
        {
            Width = Dim.Fill();
            Height = Dim.Fill();
            X = 0;
            Y = 0;
            ColorScheme = Colors.TopLevel;
            TabIndex = 0;
            Id = "Telemetry";
        }
    }
}
