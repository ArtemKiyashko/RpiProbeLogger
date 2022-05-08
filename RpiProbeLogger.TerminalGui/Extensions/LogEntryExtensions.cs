using RpiProbeLogger.BusModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;
using Microsoft.Extensions.Logging;
using NStack;

namespace RpiProbeLogger.TerminalGui.Extensions
{
    public static class LogEntryExtensions
    {
        private static readonly ColorScheme _error = new() { Normal = new Terminal.Gui.Attribute(Color.Red, Color.Black) };
        private static readonly ColorScheme _neutral = new() { Normal = new Terminal.Gui.Attribute(Color.White, Color.Black) };
        public static ColorScheme GetColorScheme(this LogEntry logEntry) => logEntry.LogLevel switch
        {
            LogLevel.Error => _error,
            LogLevel.Critical => _error,
            _ => _neutral
        };

        public static ColorScheme GetColorScheme(this ustring text) => text?.ToString()?.Split(":")[0] switch
        {
            "Error" => _error,
            "Critical" => _error,
            _ => _neutral
        };
    }
}
