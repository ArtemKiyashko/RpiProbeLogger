using HidSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpiProbeLogger.Sensors.Services
{
    public static class TemperDiveceHandlerPredicate
    {
        public static HidStream TryOpenControlDevice(this HidDevice controlDevice)
        {
            if (controlDevice.TryOpen(out var controlStream))
            {
                controlStream.Write(TemperDeviceConst.INI_COMMAND);
                controlStream.Write(TemperDeviceConst.INI1_COMMAND);
                controlStream.Write(TemperDeviceConst.INI2_COMMAND);
            }
            return controlStream;
        }
        public static HidStream TryOpenBulkDevice(this HidDevice bulkDevice)
        {
            bulkDevice.TryOpen(out var bulkStream);
            return bulkStream;
        }

        public static bool DeviceReady(HidStream controlStream, HidStream bulkStream) => controlStream is not null && bulkStream is not null; 
    }
}
