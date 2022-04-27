using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using NetMQ.Sockets;
using RpiProbeLogger.Bus;
using RpiProbeLogger.BusModels;
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
            _busReporter.BindPort(3333);
        }

        [Theory]
        [AutoData]
        public async Task ShouldSendModel_Successful(Telemetry model)
        {
            var result = await _busReporter.Send(model);
            Assert.True(result);
        }
    }
}
