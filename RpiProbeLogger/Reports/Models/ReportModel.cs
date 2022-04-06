using CsvHelper.Configuration.Attributes;
using RpiProbeLogger.Interfaces;
using Sense.Led;
using System;
using System.Numerics;

namespace RpiProbeLogger.Reports.Models
{
    public readonly record struct ReportModel(
        string Latitude,
        string Longitude,
        DateTime? DateTimeUtc,
        double? Altitude,
        double? Speed,
        double? Course,
        Vector3? FusionPose,
        Quaternion? FusionQPose,
        Vector3? Gyro,
        Vector3? Accel,
        Vector3? Compass,
        float? Pressure,
        float? PressureTemperature,
        float? Humidity,
        float? HumidityTemperature,
        double? OutsideTemperature) : IResponse
    {
        [Ignore]
        public bool Status { get; init; } = true;

        [Ignore]
        public Cell StatusPosition { get { return new(0, 4); } }
    }
}
