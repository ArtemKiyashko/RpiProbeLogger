using Terminal.Gui;

namespace RpiProbeLogger.TerminalGui
{
    public class LogView : View
    {
        public LogView() : base()
        {
            Width = Dim.Fill();
            Height = Dim.Fill();
            X = 0;
            Y = 0;
            ColorScheme = Colors.TopLevel;
            TabIndex = 1;
            Id = "Logs";
        }
    }
}
