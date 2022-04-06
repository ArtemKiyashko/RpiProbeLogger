using CsvHelper.Configuration.Attributes;
using RpiProbeLogger.Interfaces;
using Sense.Led;

namespace RpiProbeLogger.Sensors.Models
{
    public readonly record struct OutsideTemperatureResponse(
        double? OutsideTemperature) : IResponse
    {
        [Ignore]
        public bool Status => OutsideTemperature.HasValue;

        [Ignore]
        public Cell StatusPosition { get; } = new(0, 3);
    }
}
