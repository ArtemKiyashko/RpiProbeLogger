using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpiProbeLogger.TerminalGui.Settings
{
    public class TelemetryReceiverSettings
    {
        public string Ip { get; set; }
        public uint Port { get; set; }
    }
}
