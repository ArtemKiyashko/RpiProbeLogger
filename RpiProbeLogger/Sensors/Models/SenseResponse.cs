using CsvHelper.Configuration.Attributes;
using RpiProbeLogger.Interfaces;
using Sense.Led;
using System.Numerics;

namespace RpiProbeLogger.Sensors.Models
{
    public record SenseResponse : IResponse
    {
        public SenseResponse()
        {
        }

        public SenseResponse(
            Vector3? fusionPose,
            Quaternion? fusionQPose,
            Vector3? gyro,
            Vector3? accel,
            Vector3? compass,
            float? pressure,
            float? pressureTemperature,
            float? humidity,
            float? humidityTemperature)
        {
            FusionPose = fusionPose;
            FusionQPose = fusionQPose;
            Gyro = gyro;
            Accel = accel;
            Compass = compass;
            Pressure = pressure;
            PressureTemperature = pressureTemperature;
            Humidity = humidity;
            HumidityTemperature = humidityTemperature;
        }

        public Vector3? FusionPose { get; init; }
        public Quaternion? FusionQPose { get; init; }
        public Vector3? Gyro { get; init; }
        public Vector3? Accel { get; init; }
        public Vector3? Compass { get; init; }
        public float? Pressure { get; init; }
        public float? PressureTemperature { get; init; }
        public float? Humidity { get; init; }
        public float? HumidityTemperature { get; init; }

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
        public Cell StatusPosition => new(0, 2);

        public override string ToString()
        {
            return $"{FusionPose},{FusionQPose},{Gyro},{Accel},{Compass},{Pressure},{PressureTemperature},{Humidity},{HumidityTemperature}";
        }
    }
}
