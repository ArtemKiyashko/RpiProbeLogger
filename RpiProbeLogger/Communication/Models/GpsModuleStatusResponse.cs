using CsvHelper.Configuration.Attributes;
using RpiProbeLogger.Interfaces;
using Sense.Led;
using System;
using System.Collections.Generic;
using System.Text;

namespace RpiProbeLogger.Communication.Models
{
    public class GpsModuleStatusResponse : IResponse
    {
        public bool Enabled { get; set; }
        public GpsModuleModes Mode { get; set; }

        [Ignore]
        public bool Status => Enabled;

        [Ignore]
        public Cell StatusPosition => new Cell(0, 0);

        public override string ToString()
        {
            var enabled = Enabled ? "1" : "0";
            return $"{enabled},{((int)Mode)}";
        }
    }
}
