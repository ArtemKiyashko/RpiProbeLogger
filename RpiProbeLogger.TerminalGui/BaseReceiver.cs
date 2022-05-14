using NetMQ;
using NetMQ.Sockets;
using RpiProbeLogger.TerminalGui.Helpers;
using System.Text.Json;

namespace RpiProbeLogger.TerminalGui
{
    public abstract class BaseReceiver<T> where T : struct
    {
        protected readonly IDirector<T> _director;
        protected readonly SubscriberSocket _subscriber = new();

        protected BaseReceiver(IDirector<T> director)
        {
            _director = director;
        }

        protected virtual void ReceiverLoop()
        {
            while (true)
            {
                var msg = _subscriber.ReceiveFrameString();
                T model = JsonSerializer.Deserialize<T>(msg);
                _director.Refresh(model);
            }
        }
    }
}
