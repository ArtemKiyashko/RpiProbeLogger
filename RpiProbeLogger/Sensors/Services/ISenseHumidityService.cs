using Sense.RTIMU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpiProbeLogger.Sensors.Services
{
    public interface ISenseHumidityService : IDisposable
    {
        RTHumidityData Read();
    }
}
