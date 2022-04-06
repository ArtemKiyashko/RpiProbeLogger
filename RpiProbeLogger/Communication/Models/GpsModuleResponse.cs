using CsvHelper.Configuration.Attributes;
using RpiProbeLogger.Interfaces;
using Sense.Led;
using System;

namespace RpiProbeLogger.Communication.Models
{
    public readonly record struct GpsModuleResponse(
        string Latitude,
        string Longitude,
        DateTime? DateTimeUtc,
        double Altitude,
        double Speed,
        double Course) : IResponse
    {
        [Ignore]
        public bool Status => DateTimeUtc.HasValue;

        [Ignore]
        public Cell StatusPosition { get { return new(0, 1); } }
    }
}
