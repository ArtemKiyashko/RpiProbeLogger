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
        public BusReporter(PublisherSocket publisherSocket) => _publisherSocket = publisherSocket;

        public void BindPort(uint port) => _publisherSocket.Bind($"tcp://*:{port}");

        public void Dispose() => _publisherSocket.Dispose();

        public async Task<bool> Send<T>(T model)
        {
            var jsonModel = JsonSerializer.Serialize(model);
            var sendTask = Task.Run(() => SendMessage(jsonModel));
            return await sendTask;
        }

        private bool SendMessage(string jsonModel) => 
            _publisherSocket
                .TrySendFrame(TimeSpan.FromSeconds(1), jsonModel);
    }
}
