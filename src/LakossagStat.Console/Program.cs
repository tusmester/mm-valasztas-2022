using System.Threading.Tasks;
using LakossagStat.Data;
using LakossagStat.Data.Loaders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace LakossagStat.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //var path = "./lakossag.xls";
            var path = "https://www.valasztas.hu/telepulesek-lakossag-es-valasztopolgarszama";
            var options = new DataLoaderOptions { Path = path };

            //var loader = new FileDataLoader(Options.Create(options), NullLoggerFactory.Instance.CreateLogger<FileDataLoader>());
            //var loader = new UrlDataLoader(Options.Create(options), NullLoggerFactory.Instance.CreateLogger<UrlDataLoader>());
            var loader = new HtmlDataLoader(Options.Create(options), NullLoggerFactory.Instance.CreateLogger<HtmlDataLoader>());

            var data = await loader.LoadAsync();
            var analyzer = new DataAnalyzer(data);

            analyzer.Analyze();

            var json = JsonConvert.SerializeObject(data);
        }
    }
}
