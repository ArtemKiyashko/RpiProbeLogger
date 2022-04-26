using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpiProbeLogger.BusModels
{
    public readonly record struct LogEntry(
        LogLevel LogLevel,
        string UserMassage,
        string ExceptionMessage,
        string StackTrace);
}
