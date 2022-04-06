using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using RpiProbeLogger.Communication.Models;
using System.Globalization;
using System.IO;

namespace RpiProbeLogger.Reports.Services
{
    public class ReportCsvHandler : IReportFileHandler
    {
        private StreamWriter _streamWriter;
        private CsvWriter _csvWriter;
        private readonly ILogger<ReportCsvHandler> _logger;

        public ReportCsvHandler(ILogger<ReportCsvHandler> logger)
        {
            _logger = logger;
        }

        public bool CreateFile<T>(GpsModuleResponse gpsModuleResponse)
        {
            var fileName = $"{gpsModuleResponse.DateTimeUtc?.Date:ddMMyyyy}.csv";

            if (File.Exists(fileName))
            {
                _logger.LogInformation($"Report file chosen: {fileName}");
                return true;
            }

            _streamWriter = new StreamWriter(fileName, true)
            {
                AutoFlush = true
            };

            _csvWriter = new CsvWriter(_streamWriter, new CsvConfiguration(CultureInfo.InvariantCulture));
            _csvWriter.WriteHeader<T>();
            _csvWriter.NextRecord();

            _logger.LogInformation($"Report file created: {fileName}");
            return true;
        }

        protected virtual void Dispose(bool dispose)
        {
            _csvWriter?.Dispose();
            _streamWriter?.Dispose();
            _csvWriter = null;
            _streamWriter = null;
        }
        public void Dispose()
        {
            Dispose(true);
        }

        public void WriteRecord<T>(T record)
        {
            _csvWriter.WriteRecord(record);
            _csvWriter.NextRecord();
        }
    }
}
