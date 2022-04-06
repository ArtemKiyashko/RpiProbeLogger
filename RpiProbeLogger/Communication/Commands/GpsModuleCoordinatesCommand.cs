using Microsoft.Extensions.Logging;
using RpiProbeLogger.Communication.Models;
using RpiProbeLogger.Led.Services;
using System;
using System.IO.Ports;
using System.Linq;

namespace RpiProbeLogger.Communication.Commands
{
    public class GpsModuleCoordinatesCommand
    {
        private const string BASE_COMMAND = "AT+CGNSSINFO";
        private const string TIME_COMMAND = "=1";
        private readonly SerialPort _serialPort;
        private readonly ILogger<GpsModuleCoordinatesCommand> _logger;
        private readonly IStatusReportService _statusReportService;

        public delegate void CoordinatesHandler(GpsModuleResponse gpsModuleResponse);
        public event CoordinatesHandler OnCoordinatesReceived;

        public GpsModuleCoordinatesCommand(SerialPort serialPort,
            ILogger<GpsModuleCoordinatesCommand> logger,
            IStatusReportService statusReportService)
        {
            _serialPort = serialPort;
            _logger = logger;
            _statusReportService = statusReportService;
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
                var response = FormatResponse(ParseCoordinatesResponse(rawResponse));
                _statusReportService.DisplayStatus(response);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing coordinates");
                _statusReportService.DisplayStatus<GpsModuleResponse>(default);
            }
            return default;
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

        private static string[] ParseCoordinatesResponse(string rawResponse) =>
            rawResponse
                .Split(Environment.NewLine)
                .FirstOrDefault(s => s.StartsWith("+CGNSSINFO:"))?
                .Replace("\r", "")
                .Replace("+CGNSSINFO:", "")
                .Trim()
                .Split(',');

        private static GpsModuleResponse FormatResponse(string[] parsedResponse) =>
            new()
            {
                Latitude = $"{parsedResponse[5]}{double.Parse(parsedResponse[4]) / 100}",
                Longitude = $"{parsedResponse[7]}{double.Parse(parsedResponse[6]) / 100}",
                DateTimeUtc = DateTime.ParseExact($"{parsedResponse[8]} {parsedResponse[9]}", "ddMMyy HHmmss.f", null),
                Altitude = double.Parse(parsedResponse[10]),
                Speed = double.Parse(parsedResponse[11]),
                Course = double.Parse(parsedResponse[12])
            };
    }
}
