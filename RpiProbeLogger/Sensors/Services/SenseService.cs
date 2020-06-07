using Microsoft.Extensions.Logging;
using RpiProbeLogger.Led.Services;
using RpiProbeLogger.Sensors.Models;
using Sense.RTIMU;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RpiProbeLogger.Sensors.Services
{
    public class SenseService
    {
        private readonly ILogger<SenseService> _logger;
        private readonly RTIMU _senseCommon;
        private readonly RTPressure _sensePressure;
        private readonly RTHumidity _senseHumidity;
        private readonly StatusReportService _statusReportService;

        public SenseService(ILogger<SenseService> logger,
            RTIMU senseCommon,
            RTPressure sensePressure,
            RTHumidity senseHumidity,
            StatusReportService statusReportService)
        {
            _logger = logger;
            _senseCommon = senseCommon;
            _sensePressure = sensePressure;
            _senseHumidity = senseHumidity;
            _statusReportService = statusReportService;
        }

        public SenseResponse GetSensorsData()
        {
            var response = new SenseResponse();
            try
            {
                var imu = _senseCommon.GetData();
                var pressure = _sensePressure.Read();
                var humidity = _senseHumidity.Read();
                response = new SenseResponse
                {
                    FusionPose = imu.FusionPoseValid ? imu.FusionPose : (Vector3?)null,
                    FusionQPose = imu.FusionQPoseValid ? imu.FusionQPose : (Quaternion?)(null),
                    Gyro = imu.GyroValid ? imu.Gyro : (Vector3?)null,
                    Accel = imu.AccelValid ? imu.Accel : (Vector3?)null,
                    Compass = imu.CompassValid ? imu.Compass : (Vector3?)null,
                    Pressure = pressure.PressureValid ? pressure.Pressure : (float?)null,
                    PressureTemperature = pressure.TemperatureValid ? pressure.Temperatur : (float?)null,
                    Humidity = humidity.HumidityValid ? humidity.Humidity : (float?)null,
                    HumidityTemperature = humidity.TemperatureValid ? humidity.Temperatur : (float?)null
                };
                _statusReportService.DisplayStatus(response);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading Sense data");
                _statusReportService.DisplayStatus(response);
            }
            return null;
        }
    }
}
