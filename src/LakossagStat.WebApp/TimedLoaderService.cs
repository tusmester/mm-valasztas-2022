using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LakossagStat.Data;
using LakossagStat.Data.Loaders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LakossagStat.WebApp
{
    public class TimedLoaderService : IHostedService, IDisposable
    {
        public static LakossagData LakossagData { get; set; } = new LakossagData();

        private readonly IDataLoader _loader;
        private readonly ILogger<TimedLoaderService> _logger;
        private Timer _timer = null!;
        private readonly object _timerSync = new object();

        public TimedLoaderService(IDataLoader loader, ILogger<TimedLoaderService> logger)
        {
            _loader = loader;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromHours(6));

            _logger.LogInformation("Timed Loader Service is running.");

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            lock (_timerSync)
            {
                _logger.LogInformation("Timed Loader Service is working.");

                var data = _loader.LoadAsync().GetAwaiter().GetResult();
                var analyzer = new DataAnalyzer(data);

                analyzer.Analyze();

                LakossagData = data;

                var json = JsonConvert.SerializeObject(data, Formatting.Indented);

                const string fileParent = "./wwwroot/data";
                var filePath = $"{fileParent}/lakossag-data.json";
                if (!Directory.Exists(fileParent))
                    Directory.CreateDirectory(fileParent);

                File.WriteAllText(filePath, json, Encoding.UTF8);

                _logger.LogInformation("Timed Loader Service updated the data file.");
            }
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
