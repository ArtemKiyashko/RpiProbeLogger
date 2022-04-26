using NetMQ;
using NetMQ.Sockets;
using System;
using System.Threading.Tasks;
using System.Text.Json;

namespace RpiProbeLogger.Bus
{
    public class BusReporter : IBusReporter, IDisposable
    {
        private readonly PublisherSocket _publisherSocket;
        public BusReporter(string bindAddress)
        {
            _publisherSocket = new PublisherSocket();
            _publisherSocket.Bind(bindAddress);
        }

        public void Dispose()
        {
            _publisherSocket.Dispose();
        }

        public async Task<bool> Send<T>(T model, string topic)
        {
            var jsonModel = JsonSerializer.Serialize(model);
            var sendTask = Task.Run(() => SendMessage(jsonModel, topic));
            return await sendTask;
        }

        private bool SendMessage(string jsonModel, string topic)
        {
            return _publisherSocket
                .SendMoreFrame(topic)
                .TrySendFrame(TimeSpan.FromSeconds(1), jsonModel);
        }
    }
}
