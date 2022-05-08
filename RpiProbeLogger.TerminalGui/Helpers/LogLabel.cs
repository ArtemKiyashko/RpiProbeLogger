using NStack;
using RpiProbeLogger.TerminalGui.Extensions;
using Terminal.Gui;

namespace RpiProbeLogger.TerminalGui.Helpers
{
    public class LogLabel : Label
    {
        public override ustring Text { get => base.Text; 
            set 
            {
                ColorScheme = value.GetColorScheme();
                base.Text = value;
            } 
        }
    }
}
