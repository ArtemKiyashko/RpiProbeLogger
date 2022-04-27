using NetMQ;
using NetMQ.Sockets;
using RpiProbeLogger.BusModels;
using System.Text.Json;

using (var subscriber = new SubscriberSocket())
{
    subscriber.Connect("tcp://127.0.0.1:3333");
    subscriber.SubscribeToAnyTopic();

    while (true)
    {
        var msg = subscriber.ReceiveFrameString();
        var telemetry = JsonSerializer.Deserialize<Telemetry>(msg);
        Console.WriteLine("From Publisher: {0}", telemetry);
    }
}