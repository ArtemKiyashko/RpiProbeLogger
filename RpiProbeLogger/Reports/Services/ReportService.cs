﻿using Microsoft.Extensions.Logging;
using RpiProbeLogger.Communication.Models;
using RpiProbeLogger.Led.Services;
using RpiProbeLogger.Reports.Models;
using RpiProbeLogger.Sensors.Models;
using Sense.Led;
using System;
using System.Threading.Tasks;

namespace RpiProbeLogger.Reports.Services
{
    public class ReportService : IReportService
    {
        private readonly ILogger<ReportService> _logger;
        private readonly IStatusReportService _statusReportService;
        private readonly IReportFileHandler _reportFileHandler;

        public bool ReportFileCreated { get; private set; }

        public ReportService(
            ILogger<ReportService> logger,
            IStatusReportService statusReportService,
            IReportFileHandler reportFileHandler)
        {
            _logger = logger;
            _statusReportService = statusReportService;
            _reportFileHandler = reportFileHandler;
        }

        public Task<ReportModel> WriteReport(SenseResponse senseResponse, GpsModuleResponse gpsModuleResponse, OutsideTemperatureResponse outsideTemperatureResponse)
        {
            ReportModel failModel = new() { Status = false };
            try
            {
                ReportFileCreated = _reportFileHandler.CreateFile<ReportModel>(gpsModuleResponse);
                var record = MapToReportModel(senseResponse, gpsModuleResponse, outsideTemperatureResponse);
                _reportFileHandler.WriteRecord(record);
                _statusReportService.DisplayStatus(record);
                return Task.FromResult(record);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error writing report");
                _statusReportService.DisplayStatus(failModel);
            }
            return Task.FromResult(failModel);
        }

        private static ReportModel MapToReportModel(SenseResponse senseResponse, GpsModuleResponse gpsModuleResponse, OutsideTemperatureResponse outsideTemperatureResponse)
            => new()
            {
                Latitude = gpsModuleResponse.Latitude,
                Longitude = gpsModuleResponse.Longitude,
                DateTimeUtc = gpsModuleResponse.DateTimeUtc,
                Altitude = gpsModuleResponse.Altitude,
                Speed = gpsModuleResponse.Speed,
                Course = gpsModuleResponse.Course,
                FusionPose = senseResponse.FusionPose,
                FusionQPose = senseResponse.FusionQPose,
                Gyro = senseResponse.Gyro,
                Accel = senseResponse.Accel,
                Compass = senseResponse.Compass,
                Pressure = senseResponse.Pressure,
                PressureTemperature = senseResponse.PressureTemperature,
                Humidity = senseResponse.Humidity,
                HumidityTemperature = senseResponse.HumidityTemperature,
                OutsideTemperature = outsideTemperatureResponse.OutsideTemperature,
                Status = true
            };
    }
}
