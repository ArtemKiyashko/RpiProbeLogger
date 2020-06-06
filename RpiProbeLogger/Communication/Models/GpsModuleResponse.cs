using System;
using System.Collections.Generic;
using System.Text;

namespace RpiProbeLogger.Communication.Models
{
    public class GpsModuleResponse
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public DateTime DateTimeUtc { get; set; }
        public double Altitude { get; set; }
        public double Speed { get; set; }
        public double Course { get; set; }
    }
}
