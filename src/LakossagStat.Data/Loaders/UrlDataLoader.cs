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
        public UrlDataLoader(IOptions<DataLoaderOptions> options, ILogger<UrlDataLoader> logger) :
            base(options, logger)
        { }

        public override Task<LakossagData> LoadAsync()
        {
            return LoadFromUrlAsync(Options.Path);
        }

        protected async Task<LakossagData> LoadFromUrlAsync(string url)
        {
            var uri = new Uri(url);
            var client = new HttpClient();
            var response = await client.GetAsync(uri);
            const string fileParent = "./App_Data/Archive";
            var filePath = $"{fileParent}/lakossag-data-{DateTime.UtcNow:yyyy-MM-dd}.xls";
            if (!Directory.Exists(fileParent))
                Directory.CreateDirectory(fileParent);
            
            //UNDONE: skip if the file already exists (overwrite flag in config)

            await using (var fs = new FileStream(filePath, FileMode.Create))
            {
                await response.Content.CopyToAsync(fs);
            }

            return await LoadFromFileAsync(filePath);
        }
    }
}
