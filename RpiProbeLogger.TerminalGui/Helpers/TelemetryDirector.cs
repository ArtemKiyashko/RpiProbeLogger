using NStack;
using RpiProbeLogger.BusModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace RpiProbeLogger.TerminalGui.Helpers
{
    public class TelemetryDirector : IDirector<Telemetry>
    {
        private TelemetryViewBuilder _viewBuilder;
        private IDictionary<ustring, Label>? _view;

        public void Refresh(Telemetry telemetry)
        {
            if (_view is null) throw new ArgumentNullException("View not initialized. Run Setup() method first.");

            foreach (var property in typeof(Telemetry).GetProperties())
            {
                if (_view.TryGetValue(property.Name, out Label? field))
                {
                    var value = property.GetValue(telemetry);
                    field.Text = value?.ToString() ?? "NO DATA";
                    field.SetNeedsDisplay();
                }
            }
        }

        public void Setup(View container)
        {
            _viewBuilder = new TelemetryViewBuilder(container);
            _view = _viewBuilder
                        .AddLatitude()
                        .AddLongitude()
                        .AddDateTime()
                        .AddAltitude()
                        .AddSpeed()
                        .AddCourse()
                        .AddPressure()
                        .AddPressureTemperature()
                        .AddHumidity()
                        .AddHumidityTemperature()
                        .AddOutsideTemperature()
                        .Build();
        }
    }
}
