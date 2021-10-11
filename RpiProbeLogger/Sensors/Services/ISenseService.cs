using RpiProbeLogger.Sensors.Models;

namespace RpiProbeLogger.Sensors.Services
{
    public interface ISenseService
    {
        public SenseResponse GetSensorsData();
    }
}