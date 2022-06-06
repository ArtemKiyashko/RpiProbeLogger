using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace RpiProbeLogger.TerminalGui.Helpers
{
    public interface IDirector<T>
    {
        public void Setup(View container);
        public void Refresh(T telemetry);

        public event EventHandler OnRefresh;
    }
}
