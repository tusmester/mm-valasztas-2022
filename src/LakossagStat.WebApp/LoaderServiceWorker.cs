using System;
using System.IO;
using System.Text;
using LakossagStat.Data;
using LakossagStat.Data.Loaders;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LakossagStat.WebApp
{
    internal class LoaderServiceWorker
    {
        const string ParentPath = "./wwwroot/data";
        const string FilePath = ParentPath + "/lakossag-data.json";
        private const int RefreshPeriodHours = 12;

        private readonly IDataLoader _loader;
        private readonly ILogger _logger;
        private readonly object _timerSync = new object();

        public LoaderServiceWorker(IDataLoader loader, ILogger<LoaderServiceWorker> logger)
        {
            _loader = loader;
            _logger = logger;
        }

        public void EnsureData()
        {
            //UNDONE: finalize save period
            if (DataStore.LastRefresh > DateTime.UtcNow.AddHours(-RefreshPeriodHours))
            {
                _logger.LogTrace($"Skipping data load. Last refresh date: {DataStore.LastRefresh}");
                return;
            }

            lock (_timerSync)
            {
                // if refresh already happened on another thread
                if (DataStore.LastRefresh > DateTime.UtcNow.AddHours(-RefreshPeriodHours))
                    return;

                try
                {
                    if (!Directory.Exists(ParentPath))
                        Directory.CreateDirectory(ParentPath);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error accessing or creating folder {ParentPath}: {ex.Message}", ex);
                    return;
                }
                
                _logger.LogInformation("Loader Service is working.");

                LakossagData data;

                try
                {
                    data = _loader.LoadAsync().GetAwaiter().GetResult() ?? new LakossagData();

                    _logger.LogTrace("Data loaded. Analyzing...");

                    var analyzer = new DataAnalyzer(data);
                    analyzer.Analyze();

                    DataStore.LakossagData = data;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error loading or analyzing data: {ex.Message}", ex);
                    return;
                }

                _logger.LogTrace("Serializing data to json...");

                try
                {
                    var json = JsonConvert.SerializeObject(data, Formatting.Indented);

                    File.WriteAllText(FilePath, json, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error serializing or saving json data to {FilePath}: {ex.Message}", ex);
                    return;
                }

                _logger.LogInformation("Loader Service updated the data file.");

                DataStore.LastRefresh = DateTime.UtcNow;
            }
        }
    }
}
