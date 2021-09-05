using RpiProbeLogger.Interfaces;

namespace RpiProbeLogger.Led.Services
{
    public interface IStatusReportService
    {
        void Clear();
        bool DisplayStatus<T>(T status) where T : IResponse;
    }
}