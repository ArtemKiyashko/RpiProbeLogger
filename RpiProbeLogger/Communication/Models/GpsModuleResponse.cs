using CsvHelper.Configuration.Attributes;
using RpiProbeLogger.Interfaces;
using Sense.Led;
using System;
using System.Collections.Generic;
using System.Text;

namespace RpiProbeLogger.Communication.Models
{
    public class GpsModuleResponse : IResponse
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public DateTime DateTimeUtc { get; set; }
        public double Altitude { get; set; }
        public double Speed { get; set; }
        public double Course { get; set; }

        [Ignore]
        public bool Status => !string.IsNullOrEmpty(Latitude) &&
                                !string.IsNullOrEmpty(Longitude);

        [Ignore]
        public Cell StatusPosition => new Cell(0, 1);
    }
}
