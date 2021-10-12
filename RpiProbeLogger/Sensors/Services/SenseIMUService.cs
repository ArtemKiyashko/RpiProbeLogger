using Sense.RTIMU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpiProbeLogger.Sensors.Services
{
    public class SenseIMUService : ISenseIMUService
    {
        private readonly RTIMU _senseCommon;

        public SenseIMUService(RTIMU senseCommon)
        {
            _senseCommon = senseCommon;
        }

        public void Dispose() => _senseCommon.Dispose();

        public RTIMUData GetData() => _senseCommon.GetData();
    }
}
