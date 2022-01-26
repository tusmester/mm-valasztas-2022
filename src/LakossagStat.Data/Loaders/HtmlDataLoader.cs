using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LakossagStat.Data.Loaders
{
    public class HtmlDataLoader : UrlDataLoader
    {
        private const string LinkPattern = "<a href=\"(?<url>[^\\\"]+)\" target=\"_blank\" class=\"nvi-document-list-item\">";

        private readonly IHttpClientFactory _httpClientFactory;

        public HtmlDataLoader(IHttpClientFactory httpClientFactory, IOptions<DataLoaderOptions> options, 
            ILogger<HtmlDataLoader> logger) : base(httpClientFactory, options, logger)
        {
            _httpClientFactory = httpClientFactory;
        }

        public override async Task<LakossagData> LoadAsync()
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
                Logger.LogTrace($"Loading html from {Options.Path}");

                // load the webpage
                var uri = new Uri(Options.Path);
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(uri);
                var responseHtml = await response.Content.ReadAsStringAsync();

                // parse the html and find the url
                var match = Regex.Match(responseHtml, LinkPattern, RegexOptions.Multiline);
                var url = match.Groups["url"].Value;

                Logger.LogTrace($"Html loaded, XLS url found: {url}");

                return await LoadFromUrlAsync(url);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error loading or parsing html: {ex.Message}", ex);
            }

            // after error: fallback to existing file if exists
            return await LoadFromFileAsync(filePath).ConfigureAwait(false);
        }
    }
}
