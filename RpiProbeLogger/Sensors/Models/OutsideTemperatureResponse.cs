using CsvHelper.Configuration.Attributes;
using RpiProbeLogger.Interfaces;
using Sense.Led;
using System;
using System.Collections.Generic;
using System.Text;

namespace RpiProbeLogger.Sensors.Models
{
    public record OutsideTemperatureResponse : IResponse
    {
        public OutsideTemperatureResponse()
        {
        }

        public OutsideTemperatureResponse(double? outsideTemperature)
        {
            OutsideTemperature = outsideTemperature;
        }

        public double? OutsideTemperature { get; init; }

        [Ignore]
        public bool Status => OutsideTemperature.HasValue;

        [Ignore]
        public Cell StatusPosition => new(0, 3);
    }
}
