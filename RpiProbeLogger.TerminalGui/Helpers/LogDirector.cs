using NStack;
using RpiProbeLogger.BusModels;
using Terminal.Gui;

namespace RpiProbeLogger.TerminalGui.Helpers
{
    public class LogDirector : IDirector<LogEntry>
    {
        private LogLabel _view;
        private LogViewBuilder _viewBuilder;

        public event EventHandler OnRefresh;

        public void Refresh(LogEntry logEntry)
        {
            if (_view is null) throw new ArgumentNullException(nameof(_view), "View not initialized. Run Setup() method first.");

            _view.Text = $"{logEntry.LogLevel}: {logEntry.UserMessage}. " +
                $"{logEntry.ExceptionMessage}: {logEntry.StackTrace}";

            OnRefresh?.Invoke(this, new());
        }

        public void Setup(View container) 
        {
            _viewBuilder = new LogViewBuilder(container);
            _view = _viewBuilder
                .AddLogView()
                .Build();
        }
    }
}
