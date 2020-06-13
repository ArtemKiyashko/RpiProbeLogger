using CsvHelper;
using Microsoft.Extensions.Logging;
using RpiProbeLogger.Communication.Models;
using RpiProbeLogger.Led.Services;
using RpiProbeLogger.Reports.Models;
using RpiProbeLogger.Sensors.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace RpiProbeLogger.Reports.Services
{
    public class ReportService : IDisposable
    {
        private StreamWriter _streamWriter;
        private CsvWriter _csvWriter;
        private readonly ILogger<ReportService> _logger;
        private readonly StatusReportService _statusReportService;
        public bool ReportFileCreated { get => _csvWriter != null; }

        public ReportService(ILogger<ReportService> logger, StatusReportService statusReportService)
        {
            _logger = logger;
            _statusReportService = statusReportService;
        }

        protected virtual void Dispose(bool dispose)
        {
            _csvWriter?.Dispose();
            _streamWriter?.Dispose();
            _csvWriter = null;
            _streamWriter = null;
        }
        public void Dispose()
        {
            Dispose(true);
        }

        public bool WriteReport(SenseResponse senseResponse, GpsModuleResponse gpsModuleResponse, double? outsideTemperature)
        {
            var record = new ReportModel();
            try
            {
                if (_csvWriter == null)
                    CreateCsv(gpsModuleResponse);
                record = MapToReportModel(senseResponse, gpsModuleResponse, outsideTemperature);
                _csvWriter.WriteRecord(record);
                _csvWriter.NextRecord();
                _statusReportService.DisplayStatus(record);
                return true;
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error writing report");
                _statusReportService.DisplayStatus(record);
            }
            return false;
        }

        private void CreateCsv(GpsModuleResponse gpsModuleResponse)
        {
            var fileName = $"{gpsModuleResponse.DateTimeUtc.Date:ddMMyyyy}.csv";
            var existingLog = File.Exists(fileName);
            _streamWriter = new StreamWriter(fileName, true);
            _streamWriter.AutoFlush = true;
            _csvWriter = new CsvWriter(_streamWriter, CultureInfo.InvariantCulture);
            _csvWriter.Configuration.NewLine = CsvHelper.Configuration.NewLine.CRLF;
            if (!existingLog)
            {
                _csvWriter.WriteHeader<ReportModel>();
                _csvWriter.NextRecord();
            }
            _logger.LogInformation($"Report file {(existingLog ? "chosen" : "created")}: {fileName}");
        }

        private ReportModel MapToReportModel(SenseResponse senseResponse, GpsModuleResponse gpsModuleResponse, double? outsideTemperature)
            => new ReportModel
            {
                Latitude = gpsModuleResponse.Latitude,
                Longitude = gpsModuleResponse.Longitude,
                DateTimeUtc = gpsModuleResponse.DateTimeUtc,
                Altitude = gpsModuleResponse.Altitude,
                Speed = gpsModuleResponse.Speed,
                Course = gpsModuleResponse.Course,
                FusionPose = senseResponse?.FusionPose,
                FusionQPose = senseResponse?.FusionQPose,
                Gyro = senseResponse?.Gyro,
                Accel = senseResponse?.Accel,
                Compass = senseResponse?.Compass,
                Pressure = senseResponse?.Pressure,
                PressureTemperature = senseResponse?.PressureTemperature,
                Humidity = senseResponse?.Humidity,
                HumidityTemperature = senseResponse?.HumidityTemperature,
                OutsideTemperature = outsideTemperature
            };
    }
}
