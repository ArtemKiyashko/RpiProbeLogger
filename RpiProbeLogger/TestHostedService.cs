using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RpiProbeLogger
{
    public class TestHostedService : IHostedService
    {
        private readonly ILogger<TestHostedService> _logger;

        public TestHostedService(ILogger<TestHostedService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    //Thread.Sleep(10);
                    throw new ArgumentException("wrongData", "paramName");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "some exception");
                }
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
