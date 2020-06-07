using CsvHelper.Configuration.Attributes;
using RpiProbeLogger.Interfaces;
using Sense.Led;
using System;
using System.Collections.Generic;
using System.Text;

namespace RpiProbeLogger.Sensors.Models
{
    public class OutsideTemperatureResponse : IResponse
    {
        public float? OutsideTemperature { get; set; }

        [Ignore]
        public bool Status => OutsideTemperature.HasValue;

        [Ignore]
        public Cell StatusPosition => new Cell(0, 3);
    }
}
