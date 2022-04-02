using Sense.RTIMU;

namespace RpiProbeLogger.Sensors.Services
{
    public class SensePressureService : ISensePressureService
    {
        private readonly RTPressure _sensePressure;

        public SensePressureService(RTPressure sensePressure)
        {
            _sensePressure = sensePressure;
        }

        public void Dispose() => _sensePressure.Dispose();

        public RTPressureData Read() => _sensePressure.Read();
    }
}
