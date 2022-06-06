using Sense.RTIMU;
using System;

namespace RpiProbeLogger.Sensors.Services
{
    public interface ISenseHumidityService : IDisposable
    {
        RTHumidityData Read();
    }
}
