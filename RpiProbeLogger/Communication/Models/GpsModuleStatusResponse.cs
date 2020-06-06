using System;
using System.Collections.Generic;
using System.Text;

namespace RpiProbeLogger.Communication.Models
{
    public class GpsModuleStatusResponse
    {
        public bool Enabled { get; set; }
        public GpsModuleModes Mode { get; set; }
        public override string ToString()
        {
            var enabled = Enabled ? "1" : "0";
            return $"{enabled},{((int)Mode).ToString()}";
        }
    }
}
