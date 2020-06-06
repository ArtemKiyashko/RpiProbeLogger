using Microsoft.Extensions.Logging;
using RpiProbeLogger.Communication.Models;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace RpiProbeLogger.Communication.Commands
{
    public class GpsModuleStatusCommand
    {
        private const string BASE_COMMAND = "AT+CGPS";
        private const string GET_STATUS_COMMAND = "?";
        private const string SET_STATUS_COMMAND = "=";
        private readonly SerialPort _serialPort;
        private readonly ILogger<GpsModuleStatusCommand> _logger;

        public GpsModuleStatusCommand(SerialPort serialPort, ILogger<GpsModuleStatusCommand> logger)
        {
            _serialPort = serialPort;
            _logger = logger;
        }

        public GpsModuleStatusResponse GetStatus()
        {
            var command = $"{BASE_COMMAND}{GET_STATUS_COMMAND}";
            _logger.LogInformation(command);
            _serialPort.WriteLine(command);
            var rawResponse = _serialPort.ReadExisting();
            _logger.LogInformation(rawResponse);
            if (rawResponse.Contains("OK"))
            {
                try
                {
                    var parsedResponse = ParseStatusResponse(rawResponse);
                    return new GpsModuleStatusResponse
                    {
                        Enabled = parsedResponse[0] == "1",
                        Mode = (GpsModuleModes)int.Parse(parsedResponse[1])
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error parsing GPS Status Response");
                }
            }
            return null;
        }

        public bool SetStatus(GpsModuleStatusResponse status)
        {
            var currentStatus = GetStatus();
            if (currentStatus.Enabled == status.Enabled && currentStatus.Mode == status.Mode)
                return true;
            var command = $"{BASE_COMMAND}{SET_STATUS_COMMAND}{status}";
            _logger.LogInformation(command);
            _serialPort.WriteLine(command);
            var rawResponse = _serialPort.ReadExisting();
            _logger.LogInformation(rawResponse);
            return rawResponse.Contains("OK");
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
