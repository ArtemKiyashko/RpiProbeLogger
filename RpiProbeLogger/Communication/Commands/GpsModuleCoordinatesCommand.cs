using Microsoft.Extensions.Logging;
using RpiProbeLogger.Communication.Models;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace RpiProbeLogger.Communication.Commands
{
    public class GpsModuleCoordinatesCommand
    {
        private const string BASE_COMMAND = "AT+CGPSINFO";
        private const string TIME_COMMAND = "=1";
        private readonly SerialPort _serialPort;
        private readonly ILogger<GpsModuleCoordinatesCommand> _logger;

        public delegate void CoordinatesHandler(GpsModuleResponse gpsModuleResponse);
        public event CoordinatesHandler OnCoordinatesReceived;

        public GpsModuleCoordinatesCommand(SerialPort serialPort, ILogger<GpsModuleCoordinatesCommand> logger)
        {
            _serialPort = serialPort;
            _logger = logger;
        }

        public void StartReceivingData()
        {
            var command = $"{BASE_COMMAND}{TIME_COMMAND}";
            _serialPort.DataReceived += DataReceived;
            _logger.LogInformation(command);
            _serialPort.WriteLine(command);
        }

        public GpsModuleResponse GetGpsData()
        {
            var command = $"{BASE_COMMAND}";
            _logger.LogInformation(command);
            _serialPort.WriteLine(command);
            var rawResponse = _serialPort.ReadExisting();
            _logger.LogInformation(rawResponse);
            try
            {
                return FormatResponse(ParseCoordinatesResponse(rawResponse));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing coordinates");
            }
            return null;
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var rawResponse = _serialPort.ReadExisting();
            _logger.LogInformation(rawResponse);
            try
            {
                var result = FormatResponse(ParseCoordinatesResponse(rawResponse));
                OnCoordinatesReceived?.Invoke(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing coordinates");
            }
        }

        private string[] ParseCoordinatesResponse(string rawResponse) =>
            rawResponse
                .Split(Environment.NewLine)
                .FirstOrDefault(s => s.StartsWith("+CGPSINFO:"))?
                .Replace("\r", "")
                .Replace("+CGPSINFO:", "")
                .Trim()
                .Split(',');

        private GpsModuleResponse FormatResponse(string[] parsedResponse) =>
            new GpsModuleResponse {
                Latitude = $"{parsedResponse[1]}{double.Parse(parsedResponse[0]) / 100}",
                Longitude = $"{parsedResponse[3]}{double.Parse(parsedResponse[2]) / 100}",
                DateTimeUtc = DateTime.ParseExact($"{parsedResponse[4]} {parsedResponse[5]}", "ddMMyy HHmmss.f", null),
                Altitude = double.Parse(parsedResponse[6]),
                Speed = double.Parse(parsedResponse[7]),
                Course = double.Parse(parsedResponse[8])
            };
    }
}
