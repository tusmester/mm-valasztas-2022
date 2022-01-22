using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LakossagStat.WebApp
{
    internal class TimedLoaderService : IHostedService, IDisposable
    {
        private readonly LoaderServiceWorker _worker;
        private readonly ILogger<TimedLoaderService> _logger;
        private Timer _timer = null!;

        public TimedLoaderService(LoaderServiceWorker worker, ILogger<TimedLoaderService> logger)
        {
            _worker = worker;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(cb => _worker.EnsureData(), null, TimeSpan.Zero,
                TimeSpan.FromHours(1));

            _logger.LogInformation("Timed Loader Service is running.");

            return Task.CompletedTask;
        }
        
        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Loader Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
