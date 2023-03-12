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
        private string _fileName;

        public ReportCsvHandler(ILogger<ReportCsvHandler> logger)
        {
            _logger = logger;
        }

        public void CreateFile<T>(GpsModuleResponse gpsModuleResponse)
        {
            _fileName = $"{gpsModuleResponse.DateTimeUtc?.Date:ddMMyyyy}.csv";

            if (File.Exists(_fileName))
            {
                _logger.LogInformation($"Report file chosen: {_fileName}");
                return;
            }

            EnsureCsvWriterCreated();

            _csvWriter.WriteHeader<T>();
            _csvWriter.NextRecord();

            _logger.LogInformation($"Report file created: {_fileName}");
            return;
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
            EnsureCsvWriterCreated();
            _csvWriter.WriteRecord(record);
            _csvWriter.NextRecord();
        }

        private void EnsureCsvWriterCreated()
        {
            if (_streamWriter is null)
                _streamWriter = new StreamWriter(_fileName, true) { AutoFlush = true };

            if (_csvWriter is null)
                _csvWriter = new CsvWriter(_streamWriter, new CsvConfiguration(CultureInfo.InvariantCulture));
        }
    }
}
