using HidSharp;
using System;
using System.Collections.Generic;

namespace RpiProbeLogger.Sensors.Services
{
    public interface IHidDeviceHandler : IDisposable
    {
        public HidStream BulkStream { get; }
        public HidStream ControlStream { get; }
        public bool OpenDevice();
        public IEnumerable<HidDevice> GetListOfDevices();
    }
}
