using RpiProbeLogger.Sensors.Models;
using System;

namespace RpiProbeLogger.Sensors.Services
{
    public interface ITemperService
    {
        OutsideTemperatureResponse ReadTemperature();
    }
}