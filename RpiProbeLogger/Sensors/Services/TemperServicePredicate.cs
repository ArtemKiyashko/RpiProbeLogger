using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpiProbeLogger.Sensors.Services
{
    public static class TemperServicePredicate
    {
        public static double? FormatResponse(byte[] rawResult) => ((rawResult[4] & 0xFF) + ((sbyte)rawResult[3] << 8)) * 0.01;
    }
}
