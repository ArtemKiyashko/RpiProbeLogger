using CsvHelper.Configuration.Attributes;
using RpiProbeLogger.Interfaces;
using Sense.Led;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RpiProbeLogger.Sensors.Models
{
    public class SenseResponse : IResponse
    {
        public Vector3? FusionPose { get; set; }
        public Quaternion? FusionQPose { get; set; }
        public Vector3? Gyro { get; set; }
        public Vector3? Accel { get; set; }
        public Vector3? Compass { get; set; }
        public float? Pressure { get; set; }
        public float? PressureTemperature { get; set; }
        public float? Humidity { get; set; }
        public float? HumidityTemperature { get; set; }

        [Ignore]
        public bool Status => FusionPose.HasValue &&
                                FusionQPose.HasValue &&
                                Gyro.HasValue &&
                                Accel.HasValue &&
                                Compass.HasValue &&
                                Pressure.HasValue &&
                                PressureTemperature.HasValue &&
                                Humidity.HasValue &&
                                HumidityTemperature.HasValue;

        [Ignore]
        public Cell StatusPosition => new Cell(0,2);

        public override string ToString()
        {
            return $"{FusionPose},{FusionQPose},{Gyro},{Accel},{Compass},{Pressure},{PressureTemperature},{Humidity},{HumidityTemperature}";
        }
    }
}
