using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LakossagStat.Data.Loaders
{
    public class FileDataLoader : DataLoaderBase
    {
        protected DataLoaderOptions Options { get; }

        public FileDataLoader(IOptions<DataLoaderOptions> options, ILogger<FileDataLoader> logger) : base(logger)
        {
            Options = options.Value;
        }

        public override Task<LakossagData> LoadAsync()
        {
            return LoadFromFileAsync(Options.Path);
        }

        protected async Task<LakossagData> LoadFromFileAsync(string path)
        {
            if (!File.Exists(path))
                throw new InvalidOperationException($"File {Options.Path} does not exist.");

            //UNDONE: can we call this in an upper layer once?
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            try
            {
                await using var stream = File.Open(path, FileMode.Open, FileAccess.Read);
                using var reader = ExcelReaderFactory.CreateReader(stream);

                return Load(reader);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error loading XLS file {path}: {ex.Message}", ex);
            }

            return null;
        }
    }
}
