using RpiProbeLogger.Communication.Models;
using System;

namespace RpiProbeLogger.Reports.Services
{
    public interface IReportFileHandler : IDisposable
    {
        bool CreateFile<T>(GpsModuleResponse gpsModuleResponse);
        void WriteRecord<T>(T record);
    }
}
