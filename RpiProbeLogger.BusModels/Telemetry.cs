using System.Numerics;

namespace RpiProbeLogger.BusModels
{
    public readonly record struct Telemetry(
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
        double? OutsideTemperature);
}