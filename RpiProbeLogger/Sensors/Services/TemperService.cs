﻿using Microsoft.Extensions.Logging;
using RpiProbeLogger.Led.Services;
using RpiProbeLogger.Sensors.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpiProbeLogger.Sensors.Services
{
    public class TemperService : ITemperService
    {
        private readonly ILogger<TemperService> _logger;
        private readonly IStatusReportService _statusReportService;
        private readonly IHidDeviceHandler _hidDeviceHandler;

        public TemperService(
            ILogger<TemperService> logger,
            IStatusReportService statusReportService,
            IHidDeviceHandler hidDeviceHandler)
        {
            _logger = logger;
            _statusReportService = statusReportService;
            _hidDeviceHandler = hidDeviceHandler;
        }

        public OutsideTemperatureResponse ReadTemperature()
        {
            try
            {
                _hidDeviceHandler.BulkStream.Write(TemperDeviceConst.TEMP_COMMAND);
                var rawResult = _hidDeviceHandler.BulkStream.Read();
                OutsideTemperatureResponse response = new(TemperServicePredicate.FormatResponse(rawResult));
                _statusReportService.DisplayStatus(response);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error reading outside temperature", ex);
                _statusReportService.DisplayStatus<OutsideTemperatureResponse>(new());
            }
            return null;
        }
    }
}
