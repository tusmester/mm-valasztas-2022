using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LakossagStat.Data;
using LakossagStat.Data.Loaders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LakossagStat.Console
{
    class Program
    {
        const string ParentPath = "./App_Data";
        const string FilePathSmall = ParentPath + "/lakossag-data.json";
        const string FilePathBig = ParentPath + "/lakossag-data-all.json";

        static async Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();

            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            var loader = host.Services.GetRequiredService<IDataLoader>();

            logger.LogInformation($"Loading data...");

            var data = await loader.LoadAsync();

            logger.LogInformation($"Analyzing data...");

            var analyzer = new DataAnalyzer(data);

            analyzer.Analyze();

            logger.LogInformation($"Analyzing finished.");

            var json = JsonConvert.SerializeObject(data, Formatting.Indented);

            if (!Directory.Exists(ParentPath))
                Directory.CreateDirectory(ParentPath);

            // temporarily cleat the all list
            var origAllList = data.AllItems;
            data.AllItems = Array.Empty<OevkData>();

            // save the small file
            var jsonSmall = JsonConvert.SerializeObject(data, Formatting.Indented);
            await File.WriteAllTextAsync(FilePathSmall, jsonSmall, Encoding.UTF8);

            logger.LogInformation($"Data is saved to {FilePathSmall}.");

            data.AllItems = origAllList;

            // save the big file
            var jsonBig = JsonConvert.SerializeObject(data, Formatting.Indented);
            await File.WriteAllTextAsync(FilePathBig, jsonBig, Encoding.UTF8);

            logger.LogInformation($"Data is saved to {FilePathBig}.");
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hb, services) => services
                    .AddLogging(logging =>
                    {
                        logging.AddConsole();
                        logging.AddFile("Logs/mm-voksturizmus-{Date}.txt", LogLevel.Trace);
                    })
                    // configure feature-specific options
                    .Configure<DataLoaderOptions>(hb.Configuration.GetSection("DataLoader"))
                    .AddSingleton<IDataLoader, HtmlDataLoader>()
                    // http client
                    .AddHttpClient(Microsoft.Extensions.Options.Options.DefaultName)
                    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                    {
                        ClientCertificateOptions = ClientCertificateOption.Manual,
                        ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
                    })
                );
    }
}
