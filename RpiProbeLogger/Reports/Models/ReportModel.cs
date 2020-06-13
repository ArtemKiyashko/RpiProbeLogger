using CsvHelper.Configuration.Attributes;
using RpiProbeLogger.Interfaces;
using Sense.Led;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RpiProbeLogger.Reports.Models
{
    public class ReportModel : IResponse
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public DateTime DateTimeUtc { get; set; }
        public double Altitude { get; set; }
        public double Speed { get; set; }
        public double Course { get; set; }
        public Vector3? FusionPose { get; set; }
        public Quaternion? FusionQPose { get; set; }
        public Vector3? Gyro { get; set; }
        public Vector3? Accel { get; set; }
        public Vector3? Compass { get; set; }
        public float? Pressure { get; set; }
        public float? PressureTemperature { get; set; }
        public float? Humidity { get; set; }
        public float? HumidityTemperature { get; set; }
        public double? OutsideTemperature { get; set; }

        [Ignore]
        public bool Status => !string.IsNullOrEmpty(Latitude) &&
                                !string.IsNullOrEmpty(Longitude);

        [Ignore]
        public Cell StatusPosition => new Cell(0,4);
    }
}
