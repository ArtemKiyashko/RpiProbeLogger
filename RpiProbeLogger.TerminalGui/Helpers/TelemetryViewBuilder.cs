using NStack;
using Terminal.Gui;
using RpiProbeLogger.TerminalGui.Extensions;
using RpiProbeLogger.BusModels;
using System.Diagnostics.CodeAnalysis;

namespace RpiProbeLogger.TerminalGui.Helpers
{
    public class TelemetryViewBuilder
    {
        private readonly IDictionary<ustring, Label> _views = new Dictionary<ustring, Label>();
        private readonly View _container;

        public TelemetryViewBuilder([NotNull]View container) =>
            _container = container;

        public TelemetryViewBuilder AddLatitude()
        {
            InjectField(_container, nameof(Telemetry.Latitude));
            return this;
        }

        public TelemetryViewBuilder AddLongitude()
        {
            InjectField(_container, nameof(Telemetry.Longitude));
            return this;
        }

        public TelemetryViewBuilder AddDateTime()
        {
            InjectField(_container, nameof(Telemetry.DateTimeUtc));
            return this;
        }

        public TelemetryViewBuilder AddAltitude()
        {
            InjectField(_container, nameof(Telemetry.Altitude));
            return this;
        }

        public TelemetryViewBuilder AddSpeed()
        {
            InjectField(_container, nameof(Telemetry.Speed));
            return this;
        }

        public TelemetryViewBuilder AddCourse()
        {
            InjectField(_container, nameof(Telemetry.Course));
            return this;
        }

        public TelemetryViewBuilder AddPressure()
        {
            InjectField(_container, nameof(Telemetry.Pressure));
            return this;
        }

        public TelemetryViewBuilder AddPressureTemperature()
        {
            InjectField(_container, nameof(Telemetry.PressureTemperature));
            return this;
        }

        public TelemetryViewBuilder AddHumidity()
        {
            InjectField(_container, nameof(Telemetry.Humidity));
            return this;
        }

        public TelemetryViewBuilder AddHumidityTemperature()
        {
            InjectField(_container, nameof(Telemetry.HumidityTemperature));
            return this;
        }

        public TelemetryViewBuilder AddOutsideTemperature()
        {
            InjectField(_container, nameof(Telemetry.OutsideTemperature));
            return this;
        }

        private void InjectField(View root, string label)
        {
            var lastLabelView = _views.Count == 0 ? default : _views.Last();
            if (lastLabelView.Value is null)
                _views.Add(root.AddField(label, root));
            else
                _views.Add(lastLabelView.Value.AddField(label, root));
        }

        public IDictionary<ustring, Label> Build() => _views;
    }
}
