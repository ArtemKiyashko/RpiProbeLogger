using HidSharp;
using Microsoft.Extensions.Logging;
using RpiProbeLogger.Led.Services;
using RpiProbeLogger.Sensors.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpiProbeLogger.Sensors.Services
{
    public class TemperService : IDisposable
    {
        private bool _disposed = false;
        private byte[] _iniCommand = { 0x01, 0x01 };
        private byte[] _tempCommand = { 0x01, 0x80, 0x33, 0x01, 0x00, 0x00, 0x00, 0x00 };
        private byte[] _ini1Command = { 0x01, 0x82, 0x77, 0x01, 0x00, 0x00, 0x00, 0x00 };
        private byte[] _ini2Command = { 0x01, 0x86, 0xff, 0x01, 0x00, 0x00, 0x00, 0x00 };
        private HidDevice _controlDevice;
        private HidDevice _bulkDevice;
        private bool _controlDeviceOpen;
        private bool _bulkDeviceOpen;
        private HidStream _controlStream;
        private HidStream _bulkStream;

        private const int PRODUCT_ID = 0x2107;
        private const int VENDOR_ID = 0x413D;
        private const string CONTROL_INTERFACE_NAME = "hidraw0";
        private const string BULK_INTERFACE_NAME = "hidraw1";
        private readonly ILogger<TemperService> _logger;
        private readonly IStatusReportService _statusReportService;

        public TemperService(
            ILogger<TemperService> logger,
            IStatusReportService statusReportService)
        {
            _logger = logger;
            _statusReportService = statusReportService;
            OpenDevices();
        }

        private void OpenDevices()
        {
            var temperatureInterfaces = DeviceList
                .Local
                .GetHidDevices()
                .Where(hid => hid.ProductID == PRODUCT_ID && hid.VendorID == VENDOR_ID);

            _controlDevice = temperatureInterfaces.FirstOrDefault(x => x.DevicePath.Contains(CONTROL_INTERFACE_NAME));
            _bulkDevice = temperatureInterfaces.FirstOrDefault(x => x.DevicePath.Contains(BULK_INTERFACE_NAME));

            if (_controlDevice == null || _bulkDevice == null)
            {
                _statusReportService.DisplayStatus(new OutsideTemperatureResponse());
                return;
            }
            if (_controlDevice.TryOpen(out _controlStream))
            {
                _controlDeviceOpen = true;
                _controlStream.Write(_iniCommand);
                _controlStream.Write(_ini1Command);
                _controlStream.Write(_ini2Command);
            }
            if (_bulkDevice.TryOpen(out _bulkStream))
                _bulkDeviceOpen = true;
        }

        public OutsideTemperatureResponse ReadTemperature()
        {
            if (!_controlDeviceOpen || !_bulkDeviceOpen)
                OpenDevices();
            var response = new OutsideTemperatureResponse();
            try
            {
                _bulkStream.Write(_tempCommand);
                var rawResult = _bulkStream.Read();

                response.OutsideTemperature = ((rawResult[4] & 0xFF) + ((sbyte)rawResult[3] << 8)) * 0.01;
                _statusReportService.DisplayStatus(response);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error reading outside temperature", ex);
                _statusReportService.DisplayStatus(response);
            }
            return null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TemperService() => Dispose(false);

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
