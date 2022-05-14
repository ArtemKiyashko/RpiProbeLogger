using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpiProbeLogger.Communication.Settings
{
    public class SerialPortOptions
    {
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public int ReadTimeout { get; set; }
        public int WriteTimeout { get; set; }
        public string NewLine { get; set; }
    }
}
