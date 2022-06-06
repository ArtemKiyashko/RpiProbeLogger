using RpiProbeLogger.Sensors.Models;

namespace RpiProbeLogger.Sensors.Services
{
    public interface ITemperService
    {
        OutsideTemperatureResponse ReadTemperature();
    }
}