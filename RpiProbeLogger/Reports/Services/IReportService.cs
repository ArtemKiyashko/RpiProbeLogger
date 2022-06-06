using RpiProbeLogger.Communication.Models;
using RpiProbeLogger.Reports.Models;
using RpiProbeLogger.Sensors.Models;
using System.Threading.Tasks;

namespace RpiProbeLogger.Reports.Services
{
    public interface IReportService
    {
        bool ReportFileCreated { get; }

        Task<ReportModel> WriteReport(SenseResponse senseResponse, GpsModuleResponse gpsModuleResponse, OutsideTemperatureResponse outsideTemperatureResponse);
    }
}