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
        const string FilePath = ParentPath + "/lakossag-data.json";

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

            await File.WriteAllTextAsync(FilePath, json, Encoding.UTF8).ConfigureAwait(false);

            logger.LogInformation($"Data is saved to {FilePath}.");
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
