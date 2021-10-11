using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpiProbeLogger.Sensors.Services
{
    public static class TemperDeviceConst
    {
        public const string CONTROL_INTERFACE_NAME = "hidraw0";
        public const string BULK_INTERFACE_NAME = "hidraw1";
        public static readonly byte[] INI_COMMAND = { 0x01, 0x01 };
        public static readonly byte[] TEMP_COMMAND = { 0x01, 0x80, 0x33, 0x01, 0x00, 0x00, 0x00, 0x00 };
        public static readonly byte[] INI1_COMMAND = { 0x01, 0x82, 0x77, 0x01, 0x00, 0x00, 0x00, 0x00 };
        public static readonly byte[] INI2_COMMAND = { 0x01, 0x86, 0xff, 0x01, 0x00, 0x00, 0x00, 0x00 };
    }
}
