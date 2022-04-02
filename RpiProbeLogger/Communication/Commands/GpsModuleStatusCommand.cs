using Microsoft.Extensions.Logging;
using RpiProbeLogger.Communication.Models;
using RpiProbeLogger.Led.Services;
using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace RpiProbeLogger.Communication.Commands
{
    public class GpsModuleStatusCommand
    {
        private const string BASE_COMMAND = "AT+CGPS";
        private const string GET_STATUS_COMMAND = "?";
        private const string SET_STATUS_COMMAND = "=";
        private readonly SerialPort _serialPort;
        private readonly ILogger<GpsModuleStatusCommand> _logger;
        private readonly IStatusReportService _statusReportService;

        public GpsModuleStatusCommand(SerialPort serialPort,
            ILogger<GpsModuleStatusCommand> logger,
            IStatusReportService statusReportService)
        {
            _serialPort = serialPort;
            _logger = logger;
            _statusReportService = statusReportService;
        }

        public GpsModuleStatusResponse GetStatus()
        {
            var response = new GpsModuleStatusResponse { Enabled = false };
            var command = $"{BASE_COMMAND}{GET_STATUS_COMMAND}";
            _logger.LogInformation(command);
            _serialPort.WriteLine(command);
            Thread.Sleep(500);
            var rawResponse = _serialPort.ReadExisting();
            _logger.LogInformation(rawResponse);
            if (rawResponse.Contains("OK"))
            {
                try
                {
                    var parsedResponse = ParseStatusResponse(rawResponse);
                    response = new GpsModuleStatusResponse
                    {
                        Enabled = parsedResponse[0] == "1",
                        Mode = (GpsModuleModes)int.Parse(parsedResponse[1])
                    };
                    _statusReportService.DisplayStatus(response);
                    return response;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error parsing GPS Status Response");
                    _statusReportService.DisplayStatus(response);
                }
            }
            return response;
        }

        public bool SetStatus(GpsModuleStatusResponse status)
        {
            var command = $"{BASE_COMMAND}{SET_STATUS_COMMAND}{status}";
            _logger.LogInformation(command);
            _serialPort.WriteLine(command);
            Thread.Sleep(5000);
            var rawResponse = _serialPort.ReadExisting();
            _logger.LogInformation(rawResponse);
            var response = new GpsModuleStatusResponse
            {
                Enabled = rawResponse.Contains("OK")
            };
            _statusReportService.DisplayStatus(response);
            return true;
        }

        private string[] ParseStatusResponse(string rawResponse) =>
            rawResponse
                .Split(Environment.NewLine)
                .FirstOrDefault(s => s.StartsWith("+CGPS:"))?
                .Replace("\r", "")
                .Replace("+CGPS:", "")
                .Trim()
                .Split(',');
    }
}
