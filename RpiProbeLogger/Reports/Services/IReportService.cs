using RpiProbeLogger.Communication.Models;
using RpiProbeLogger.Sensors.Models;

namespace RpiProbeLogger.Reports.Services
{
    public interface IReportService
    {
        bool ReportFileCreated { get; }

        bool WriteReport(SenseResponse senseResponse, GpsModuleResponse gpsModuleResponse, OutsideTemperatureResponse outsideTemperatureResponse);
    }
}