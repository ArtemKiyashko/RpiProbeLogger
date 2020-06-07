using CsvHelper.Configuration.Attributes;
using Sense.Led;
using System;
using System.Collections.Generic;
using System.Text;

namespace RpiProbeLogger.Interfaces
{
    public interface IResponse
    {
        public bool Status { get; }
        public Cell StatusPosition { get; }
    }
}
