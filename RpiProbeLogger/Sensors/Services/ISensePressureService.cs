using Sense.RTIMU;
using System;

namespace RpiProbeLogger.Sensors.Services
{
    public interface ISensePressureService : IDisposable
    {
        RTPressureData Read();
    }
}
