using System;
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

        public HtmlDataLoader(IOptions<DataLoaderOptions> options, ILogger<HtmlDataLoader> logger) : base(options,
            logger)
        {
        }

        public override async Task<LakossagData> LoadAsync()
        {
            // load the webpage
            var uri = new Uri(Options.Path);
            var client = new HttpClient();
            var response = await client.GetAsync(uri);
            var responseHtml = await response.Content.ReadAsStringAsync();

            // parse the html and find the url
            var match = Regex.Match(responseHtml, LinkPattern, RegexOptions.Multiline);
            var url = match.Groups["url"].Value;

            return await LoadFromUrlAsync(url);
        }
    }
}
