using CsvHelper.Configuration.Attributes;
using RpiProbeLogger.Interfaces;
using Sense.Led;
using System;
using System.Numerics;

namespace RpiProbeLogger.Reports.Models
{
    public record ReportModel : IResponse
    {
        public ReportModel()
        {
        }

        public ReportModel(
            string latitude,
            string longitude,
            DateTime? dateTimeUtc,
            double? altitude,
            double? speed,
            double? course,
            Vector3? fusionPose,
            Quaternion? fusionQPose,
            Vector3? gyro,
            Vector3? accel,
            Vector3? compass,
            float? pressure,
            float? pressureTemperature,
            float? humidity,
            float? humidityTemperature,
            double? outsideTemperature)
        {
            Latitude = latitude;
            Longitude = longitude;
            DateTimeUtc = dateTimeUtc;
            Altitude = altitude;
            Speed = speed;
            Course = course;
            FusionPose = fusionPose;
            FusionQPose = fusionQPose;
            Gyro = gyro;
            Accel = accel;
            Compass = compass;
            Pressure = pressure;
            PressureTemperature = pressureTemperature;
            Humidity = humidity;
            HumidityTemperature = humidityTemperature;
            OutsideTemperature = outsideTemperature;
        }

        public string Latitude { get; init; }
        public string Longitude { get; init; }
        public DateTime? DateTimeUtc { get; init; }
        public double? Altitude { get; init; }
        public double? Speed { get; init; }
        public double? Course { get; init; }
        public Vector3? FusionPose { get; init; }
        public Quaternion? FusionQPose { get; init; }
        public Vector3? Gyro { get; init; }
        public Vector3? Accel { get; init; }
        public Vector3? Compass { get; init; }
        public float? Pressure { get; init; }
        public float? PressureTemperature { get; init; }
        public float? Humidity { get; init; }
        public float? HumidityTemperature { get; init; }
        public double? OutsideTemperature { get; init; }

        [Ignore]
        public bool Status => !string.IsNullOrEmpty(Latitude) &&
                                !string.IsNullOrEmpty(Longitude);

        [Ignore]
        public Cell StatusPosition => new Cell(0, 4);
    }
}
