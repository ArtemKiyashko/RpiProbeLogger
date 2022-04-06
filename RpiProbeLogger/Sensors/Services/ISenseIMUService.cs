using Sense.RTIMU;
using System;

namespace RpiProbeLogger.Sensors.Services
{
    public interface ISenseIMUService : IDisposable
    {
        RTIMUData GetData();
    }
}
