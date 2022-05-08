using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using NetMQ.Sockets;
using RpiProbeLogger.Bus;
using RpiProbeLogger.BusModels;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ReportServiceTests
{
    public class BusReporterTests
    {
        private readonly IFixture _fixture;
        private readonly IBusReporter _busReporter;

        public BusReporterTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Register(() => new PublisherSocket());
            _busReporter = _fixture.Create<BusReporter>();
        }

        [Theory]
        [AutoData]
        public async Task ShouldSendModel_Successful(Telemetry model)
        {
            var result = await _busReporter.Send(model);
            Assert.True(result);
        }

        [Theory(Skip = "For manual tests")]
        [AutoData]
        public async Task ShouldSendTelmetryModels_Successful(IList<Telemetry> models)
        {
            _busReporter.BindPort(5557);
            while (true)
            foreach (var model in models)
            {
                var result = await _busReporter.Send(model);
                Assert.True(result);
                Thread.Sleep(1000);
            }
        }

        [Theory(Skip = "For manual tests")]
        [AutoData]
        public async Task ShouldSendLogModels_Successful(IList<LogEntry> models)
        {
            _busReporter.BindPort(5556);
            models.Add(new LogEntry() { LogLevel = Microsoft.Extensions.Logging.LogLevel.Error });
            while (true)
                foreach (var model in models)
                {
                    var result = await _busReporter.Send(model);
                    Assert.True(result);
                    Thread.Sleep(1000);
                }
        }

        [Theory(Skip = "For manual tests")]
        [AutoData]
        public async Task ShouldSendAllModels_Successful(IList<LogEntry> logs, IList<Telemetry> telemetry)
        {
            var logReporter = _fixture.Create<BusReporter>();
            var telemetryReporter = _fixture.Create<BusReporter>();

            logReporter.BindPort(5556);
            telemetryReporter.BindPort(5557);

            var tasks = new List<Task>
            {
                Task.Run(async () =>
                {
                    while (true)
                    {
                        foreach (var log in logs)
                        {
                            await logReporter.Send(log);
                            Thread.Sleep(70);
                        }
                    }
                }),

                Task.Run(async () =>
                {
                    while (true)
                    {
                        foreach (var t in telemetry)
                        {
                            await telemetryReporter.Send(t);
                            Thread.Sleep(300);
                        }
                    }
                })
            };

            await Task.WhenAll(tasks);
        }
    }
}
