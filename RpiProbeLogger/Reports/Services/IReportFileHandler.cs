using RpiProbeLogger.Communication.Models;
using RpiProbeLogger.Reports.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RpiProbeLogger.Reports.Services
{
    public interface IReportFileHandler : IDisposable
    {
        bool CreateFile<T>(GpsModuleResponse gpsModuleResponse);
        void WriteRecord<T>(T record);
    }
}
