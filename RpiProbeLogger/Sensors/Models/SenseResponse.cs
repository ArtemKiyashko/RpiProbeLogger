using CsvHelper.Configuration.Attributes;
using RpiProbeLogger.Interfaces;
using Sense.Led;
using System.Numerics;

namespace RpiProbeLogger.Sensors.Models
{
    public readonly record struct SenseResponse(Vector3? FusionPose,
        Quaternion? FusionQPose,
        Vector3? Gyro,
        Vector3? Accel,
        Vector3? Compass,
        float? Pressure,
        float? PressureTemperature,
        float? Humidity,
        float? HumidityTemperature) : IResponse
    {
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
        public Cell StatusPosition { get; } = new(0, 2);

        public override string ToString()
        {
            return $"{FusionPose},{FusionQPose},{Gyro},{Accel},{Compass},{Pressure},{PressureTemperature},{Humidity},{HumidityTemperature}";
        }
    }
}
