using HidSharp;
using RpiProbeLogger.Led.Services;
using RpiProbeLogger.Sensors.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RpiProbeLogger.Sensors.Services
{
    public class TemperDeviceHandler : IHidDeviceHandler
    {
        private readonly int _productId;
        private readonly int _vendorId;
        private readonly IStatusReportService _statusReportService;
        private HidStream _controlStream;
        private HidStream _bulkStream;

        public TemperDeviceHandler(
            int productId,
            int vendorId,
            IStatusReportService statusReportService)
        {
            _productId = productId;
            _vendorId = vendorId;
            _statusReportService = statusReportService;
        }

        public HidStream BulkStream => _bulkStream;

        public HidStream ControlStream => _controlStream;

        public bool _disposed = false;

        public IEnumerable<HidDevice> GetListOfDevices() =>
            DeviceList
                .Local
                .GetHidDevices()
                .Where(hid => hid.ProductID == _productId && hid.VendorID == _vendorId);

        public bool OpenDevice()
        {
            IEnumerable<HidDevice> temperatureInterfaces = GetListOfDevices();

            var controlDevice = temperatureInterfaces.FirstOrDefault(x => x.DevicePath.Contains(TemperDeviceConst.CONTROL_INTERFACE_NAME));
            var bulkDevice = temperatureInterfaces.FirstOrDefault(x => x.DevicePath.Contains(TemperDeviceConst.BULK_INTERFACE_NAME));

            if (controlDevice is null || bulkDevice is null)
            {
                _statusReportService.DisplayStatus<OutsideTemperatureResponse>(new(default));
                return false;
            }
            _controlStream = controlDevice.TryOpenControlDevice();
            _bulkStream = bulkDevice.TryOpenBulkDevice();

            return TemperDiveceHandlerPredicate.DeviceReady(_controlStream, _bulkStream);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TemperDeviceHandler() => Dispose(false);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _controlStream.Dispose();
                _bulkStream.Dispose();
            }

            _disposed = true;
        }
    }
}
