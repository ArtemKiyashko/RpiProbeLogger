using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using RpiProbeLogger.Led.Services;
using RpiProbeLogger.Sensors.Models;
using RpiProbeLogger.Sensors.Services;
using Sense.RTIMU;
using SenseServiceTests.TestData;
using System;
using Xunit;

namespace SenseServiceTests
{
    public class SenseSeriveTests
    {
        private readonly IFixture _fixture;
        private readonly ISenseService _senseService;
        private readonly Mock<IStatusReportService> _statusReportServiceMock;
        private readonly Mock<ISenseIMUService> _imuServiceMock;
        private readonly Mock<ISensePressureService> _pressureServiceMock;
        private readonly Mock<ISenseHumidityService> _humidityServiceMock;
        public SenseSeriveTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization() { ConfigureMembers = true });

            _statusReportServiceMock = _fixture.Freeze<Mock<IStatusReportService>>();
            _imuServiceMock = _fixture.Freeze<Mock<ISenseIMUService>>();
            _pressureServiceMock = _fixture.Freeze<Mock<ISensePressureService>>();
            _humidityServiceMock = _fixture.Freeze<Mock<ISenseHumidityService>>();
            _senseService = _fixture.Create<SenseService>();
        }

        [Fact]
        public void ShouldShowErrorStatus_IfException()
        {
            _imuServiceMock.Setup(imu => imu.GetData()).Throws<Exception>();
            _senseService.GetSensorsData();
            _statusReportServiceMock.Verify(r => r.DisplayStatus(It.Is<SenseResponse>(r => !r.Status)), Times.Once);
        }

        [Theory]
        [ClassData(typeof(SenseServiceValidResponseTestData))]
        public void ShouldShowSuccessStatus_IfNoException(RTIMUData imuData, RTPressureData pressureData, RTHumidityData humidityData)
        {
            _imuServiceMock.Setup(imu => imu.GetData()).Returns(imuData);
            _pressureServiceMock.Setup(pressure => pressure.Read()).Returns(pressureData);
            _humidityServiceMock.Setup(humidity => humidity.Read()).Returns(humidityData);
            _senseService.GetSensorsData();
            _statusReportServiceMock.Verify(r => r.DisplayStatus(It.Is<SenseResponse>(r => r.Status)), Times.Once);
        }

        [Fact]
        public void ShouldShowErrorStatus_IfAnyResponseContainInvalidData()
        {
            _senseService.GetSensorsData();
            _statusReportServiceMock.Verify(r => r.DisplayStatus(It.Is<SenseResponse>(r => !r.Status)), Times.Once);
        }

        [Fact]
        public void ShouldReadImuData()
        {
            _senseService.GetSensorsData();
            _imuServiceMock.Verify(imu => imu.GetData(), Times.Once);
        }

        [Fact]
        public void ShouldReadPressureData()
        {
            _senseService.GetSensorsData();
            _pressureServiceMock.Verify(pressure => pressure.Read(), Times.Once);
        }

        [Fact]
        public void ShouldReadHumidityData()
        {
            _senseService.GetSensorsData();
            _humidityServiceMock.Verify(humidity => humidity.Read(), Times.Once);
        }
    }
}
