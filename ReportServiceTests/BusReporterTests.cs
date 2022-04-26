using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using RpiProbeLogger.Bus;
using RpiProbeLogger.BusModels;
using System.Threading.Tasks;
using Xunit;

namespace ReportServiceTests
{
    public class BusReporterTests
    {
        private readonly IFixture _fixture;
        private readonly BusReporter _busReporter;

        public BusReporterTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _busReporter = _fixture.Create<BusReporter>();
        }

        [Theory]
        [AutoData]
        public async Task Test(Report model)
        {
            var result = await _busReporter.Send(model, Constants.REPORT_TOPIC_NAME);
            Assert.True(result);
        }
    }
}
