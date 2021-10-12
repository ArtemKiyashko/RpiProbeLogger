using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using FluentAssertions;
using HidSharp;
using Moq;
using RpiProbeLogger.Interfaces;
using RpiProbeLogger.Led.Services;
using RpiProbeLogger.Sensors.Models;
using RpiProbeLogger.Sensors.Services;
using System;
using System.IO;
using Xunit;

namespace TemperServiceTests
{
    public class TemperServiceTests
    {
        private readonly IFixture _fixture;
        private readonly ITemperService _temperService;
        private readonly Mock<IHidDeviceHandler> _hidDeviceHandlerMock;
        private readonly Mock<IStatusReportService> _statusReportServiceMock;
        private readonly Mock<HidStream> _hidStreamMock;

        public TemperServiceTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _hidDeviceHandlerMock = _fixture.Freeze<Mock<IHidDeviceHandler>>();
            _statusReportServiceMock = _fixture.Freeze<Mock<IStatusReportService>>();
            _hidStreamMock = _fixture.Freeze<Mock<HidStream>>();
            _hidStreamMock.SetReturnsDefault<int>(8); //need to be set to let HidStream.Read() method return non-empty byte array
            _hidDeviceHandlerMock.SetupGet(d => d.BulkStream).Returns(_hidStreamMock.Object);

            _temperService = _fixture.Create<TemperService>();
        }

        [Fact]
        public void ShouldShowErrorStatus_IfException()
        {
            _hidDeviceHandlerMock.SetupGet(handler => handler.BulkStream).Throws<Exception>();
            _temperService.ReadTemperature();
            _statusReportServiceMock.Verify(r => r.DisplayStatus(It.Is<OutsideTemperatureResponse>(r => !r.Status)), Times.Once);
        }

        [Fact]
        public void ShouldShowSuccessStatus_IfNoException()
        {
            _temperService.ReadTemperature();
            _statusReportServiceMock.Verify(r => r.DisplayStatus(It.Is<OutsideTemperatureResponse>(r => r.Status)), Times.Once);
        }

        [Fact]
        public void ShouldNotThrowException_BulkStreamError()
        {
            _hidDeviceHandlerMock.SetupGet(d => d.BulkStream).Throws<Exception>();
            _temperService.ReadTemperature();
        }

        [Fact]
        public void ShouldNotThrowException_BulkStreamWrongResponseLength()
        {
            _hidStreamMock.SetReturnsDefault<int>(2);
            _temperService.ReadTemperature();
        }

        [Fact]
        public void ShouldNotThrowException_ReportStatusError()
        {
            _statusReportServiceMock.Setup(r => r.DisplayStatus(It.Is<OutsideTemperatureResponse>(r => r.Status))).Throws<Exception>();
            _temperService.ReadTemperature();
        }
    }
}
