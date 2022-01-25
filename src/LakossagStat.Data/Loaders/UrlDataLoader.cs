using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LakossagStat.Data.Loaders
{
    public class UrlDataLoader : FileDataLoader
    {
        const string DownloadFileParent = "./App_Data/Archive";
        protected string DownloadFilePath => $"{DownloadFileParent}/lakossag-data-{DateTime.UtcNow:yyyy-MM-dd}.xls";

        private readonly IHttpClientFactory _httpClientFactory;

        public UrlDataLoader(IHttpClientFactory httpClientFactory, IOptions<DataLoaderOptions> options,
            ILogger<UrlDataLoader> logger) :
            base(options, logger)
        {
            _httpClientFactory = httpClientFactory;
        }

        public override Task<LakossagData> LoadAsync()
        {
            return LoadFromUrlAsync(Options.Path);
        }

        protected async Task<LakossagData> LoadFromUrlAsync(string url)
        {
            // if the file for today already exists, use that
            var filePath = DownloadFilePath;
            if (File.Exists(filePath) && !Options.OverwriteLocalFile)
            {
                Logger.LogInformation("Local XLS file already exists, loading data from there.");

                return await LoadFromFileAsync(filePath).ConfigureAwait(false);
            }

            try
            {
                Logger.LogTrace($"Loading XLS file from {url}");

                var uri = new Uri(url);
                var client = _httpClientFactory.CreateClient(nameof(UrlDataLoader));
                var response = await client.GetAsync(uri);

                if (!Directory.Exists(DownloadFileParent))
                {
                    Logger.LogTrace($"Creating local folder {DownloadFileParent}");
                    Directory.CreateDirectory(DownloadFileParent);
                }

                Logger.LogTrace($"Creating local file {filePath}");

                await using var fs = new FileStream(filePath, FileMode.Create);
                await response.Content.CopyToAsync(fs);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, $"Error during saving XLS file " +
                                     $"from the url {url} to the file system: {ex.Message}. " +
                                     $"Loading from local file if it exists.");
            }

            return await LoadFromFileAsync(filePath);
        }
    }
}
