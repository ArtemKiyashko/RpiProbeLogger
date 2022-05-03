using NetMQ;
using NetMQ.Sockets;
using RpiProbeLogger.TerminalGui.Helpers;
using Terminal.Gui;

Application.Init();

var mainWindow = new Window("VOSTOK")
{
    Width = Dim.Fill(),
    Height = Dim.Fill(),
    X = 0,
    Y = 0,
    ColorScheme = Colors.TopLevel
};

ITelemetryDirector telemetryDirector = new TelemetryDirector();
telemetryDirector.Setup(mainWindow);

Application.Top.Add(mainWindow);
Application.Run();

using (var subscriber = new SubscriberSocket())
{
    subscriber.Connect("tcp://127.0.0.1:5557");
    subscriber.SubscribeToAnyTopic();

    while (true)
    {
        var msg = subscriber.ReceiveFrameString();
        Console.WriteLine("From Publisher: {0}", msg);
    }
}