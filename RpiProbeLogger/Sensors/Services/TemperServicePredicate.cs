namespace RpiProbeLogger.Sensors.Services
{
    public static class TemperServicePredicate
    {
        public static double? FormatResponse(this byte[] rawResult) => ((rawResult[4] & 0xFF) + ((sbyte)rawResult[3] << 8)) * 0.01;
    }
}
