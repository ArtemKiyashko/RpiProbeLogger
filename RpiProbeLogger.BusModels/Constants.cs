using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpiProbeLogger.BusModels
{
    public static class Constants
    {
        public const string BIND_ADDRESS = "tcp://*:5556";
        public const string REPORT_TOPIC_NAME = "reports";
        public const string ERROR_TOPIC_NAME = "errors";
    }
}
