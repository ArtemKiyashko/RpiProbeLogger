using NetMQ;
using NetMQ.Sockets;

using (var subscriber = new SubscriberSocket())
{
    subscriber.Connect("tcp://127.0.0.1:5556");
    subscriber.Subscribe("errors");

    while (true)
    {
        var topic = subscriber.ReceiveFrameString();
        var msg = subscriber.ReceiveFrameString();
        Console.WriteLine("From Publisher: {0} {1}", topic, msg);
    }
}