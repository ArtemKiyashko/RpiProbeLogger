using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RJCP.IO.Ports;
using RpiProbeLogger.Communication.Commands;
using Sense;
using Sense.RTIMU;

namespace RpiProbeLogger
{
    class Program
    {

        static SerialPort _serialPort;
        static bool _continue;
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
                })
                .ConfigureLogging(logConfig =>
                {
                    logConfig.SetMinimumLevel(LogLevel.Information);
                    logConfig.AddConsole();
                })
                .Build();
            await host.RunAsync();
            return;
            for (; ; )
            {
                Console.WriteLine("waiting for debugger attach");
                if (Debugger.IsAttached) break;
                Thread.Sleep(1000);
            }

            string name;
            string message;
            StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
            Thread readThread = new Thread(Read);
            //_serialPort = new SerialPort("/dev/ttyS0", 115200, Parity.None, 8, StopBits.One);
            //_serialPort = new SerialPortStream("/dev/ttyS0", 115200);
            _serialPort = new SerialPort("/dev/ttyS0", 115200);

            //_serialPort.Handshake = Handshake.None;
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;
            _serialPort.NewLine = "\r";
            //_serialPort.DataReceived += sp_DataReceived;

            _serialPort.Open();
            _serialPort.WriteLine("AT+CGPS?");
            var result =_serialPort.ReadExisting();
            readThread.Start();
            Console.ReadKey();
            TestRTIMULib();
        }

        private static void Read()
        {
            while (true)
            {
                try
                {
                    _serialPort.WriteLine("AT+CGPSINFO");
                    Thread.Sleep(1000);
                }
                catch (TimeoutException ex) { }
            }
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
