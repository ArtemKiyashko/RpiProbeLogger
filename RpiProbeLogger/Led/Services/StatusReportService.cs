using Microsoft.Extensions.Logging;
using RpiProbeLogger.Interfaces;
using Sense.Led;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace RpiProbeLogger.Led.Services
{
    public class StatusReportService : IDisposable, IStatusReportService
    {
        private readonly ILogger<StatusReportService> _logger;
        private readonly ILedMatrix _ledMatrix;
        private IList<CellColor> _currentStatuses;
        private readonly IDictionary<bool?, Color> statusToColorMapping = new Dictionary<bool?, Color>
        {
            { true, new Color(0, 255, 0) },
            { false, new Color(255, 0, 0) },
        };

        public StatusReportService(ILogger<StatusReportService> logger, ILedMatrix ledMatrix)
        {
            _logger = logger;
            _ledMatrix = ledMatrix;
            _currentStatuses = new List<CellColor>();
            Show();
        }

        public bool DisplayStatus<T>(T status) where T : IResponse
        {
            try
            {
                var currentStatus = _currentStatuses.FirstOrDefault(c => c.Cell.Row == status.StatusPosition.Row
                                                                    && c.Cell.Column == status.StatusPosition.Column);
                _currentStatuses.Remove(currentStatus);
                _currentStatuses.Add(new CellColor(status.StatusPosition, statusToColorMapping[status.Status]));
                Show();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error displaying status on LED");
                return false;
            }
        }

        public void Clear()
        {
            _currentStatuses.Clear();
            Show();
        }

        private void Show()
        {
            var pixels = new Pixels(ImmutableList.CreateRange(_currentStatuses));
            _ledMatrix.SetPixels(pixels);
        }

        protected virtual void Dispose(bool dispose)
        {
            Clear();
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
