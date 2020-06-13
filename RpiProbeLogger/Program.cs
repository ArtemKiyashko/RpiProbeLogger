using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RpiProbeLogger.Communication.Commands;
using RpiProbeLogger.Led.Services;
using RpiProbeLogger.Reports.Services;
using RpiProbeLogger.Sensors.Services;
using Sense.RTIMU;

namespace RpiProbeLogger
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureServices((hostContext, services) => {
                    services.AddHostedService<RpiProbeHostedService>();
                    services.AddSingleton<SerialPort>((_) => { 
                        var serialPort = new SerialPort("/dev/ttyS0", 115200);
                        serialPort.ReadTimeout = 500;
                        serialPort.WriteTimeout = 500;
                        serialPort.NewLine = "\r";
                        serialPort.Open();
                        return serialPort;
                    });
                    services.AddTransient<GpsModuleStatusCommand>();
                    services.AddTransient<GpsModuleCoordinatesCommand>();
                    services.AddSingleton<RTIMUSettings>((_) => RTIMUSettings.CreateDefault());
                    services.AddSingleton<RTIMU>((provider) => {
                        var muSettings = provider.GetService<RTIMUSettings>();
                        return muSettings.CreateIMU();
                    });
                    services.AddSingleton<RTPressure>((provider) => {
                        var muSettings = provider.GetService<RTIMUSettings>();
                        return muSettings.CreatePressure();
                    });
                    services.AddSingleton<RTHumidity>((provider) => {
                        var muSettings = provider.GetService<RTIMUSettings>();
                        return muSettings.CreateHumidity();
                    });
                    services.AddTransient<SenseService>();
                    services.AddTransient<ReportService>();
                    services.AddSingleton<StatusReportService>();
                    services.AddSingleton<TemperService>();
                })
                .ConfigureLogging(logConfig =>
                {
                    logConfig.SetMinimumLevel(LogLevel.Information);
                    logConfig.AddConsole();
                })
                .Build();
            await host.RunAsync();
        }

        private static void TestRTIMULib()
        {
            using (var settings = RTIMUSettings.CreateDefault())
            using (var imu = settings.CreateIMU())
            using (var pressure = settings.CreatePressure())
            using (var humidity = settings.CreateHumidity())
            {
                while (true)
                {
                    var imuData = imu.GetData();
                    Console.WriteLine($"Timestamp: {imuData.Timestamp:O}");
                    Console.WriteLine($"FusionPose: Valid: {imuData.FusionPoseValid}, Value: {imuData.FusionPose}");
                    Console.WriteLine($"FusionQPose: Valid: {imuData.FusionQPoseValid}, Value: {imuData.FusionQPose}");
                    Console.WriteLine($"Gyro: Valid: {imuData.GyroValid}, Value: {imuData.Gyro}");
                    Console.WriteLine($"Accel: Valid: {imuData.AccelValid}, Value: {imuData.Accel}");
                    Console.WriteLine($"Compass: Valid: {imuData.CompassValid}, Value: {imuData.Compass}");
                    Console.WriteLine();

                    var pressureReadResult = pressure.Read();
                    Console.WriteLine($"Pressure valid: {pressureReadResult.PressureValid}");
                    Console.WriteLine($"Pressure: {pressureReadResult.Pressure}");
                    Console.WriteLine($"Temperature valid: {pressureReadResult.TemperatureValid}");
                    Console.WriteLine($"Temperature: {pressureReadResult.Temperatur}");
                    Console.WriteLine();

                    var humidityReadResult = humidity.Read();
                    Console.WriteLine($"Humidity valid: {humidityReadResult.HumidityValid}");
                    Console.WriteLine($"Humidity: {humidityReadResult.Humidity}");
                    Console.WriteLine($"Temperature valid: {humidityReadResult.TemperatureValid}");
                    Console.WriteLine($"Temperature: {humidityReadResult.Temperatur}");

                    Console.WriteLine("===================================================");
                    Console.ReadLine();
                }
            }
        }
    }
}
