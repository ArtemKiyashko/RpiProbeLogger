using Sense.RTIMU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpiProbeLogger.Sensors.Services
{
    public class SenseHumidityService : ISenseHumidityService
    {
        private readonly RTHumidity _senseHumidity;

        public SenseHumidityService(RTHumidity senseHumidity)
        {
            _senseHumidity = senseHumidity;
        }

        public void Dispose() => _senseHumidity.Dispose();

        public RTHumidityData Read() => _senseHumidity.Read();
    }
}
