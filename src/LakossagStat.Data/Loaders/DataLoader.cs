using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.Extensions.Logging;

namespace LakossagStat.Data.Loaders
{
    public interface IDataLoader
    {
        Task<LakossagData> LoadAsync();
    }

    public class DataLoaderOptions
    {
        public string Path { get; set; }
    }
    
    public abstract class DataLoaderBase : IDataLoader
    {
        protected readonly ILogger Logger;

        protected DataLoaderBase(ILogger logger)
        {
            Logger = logger;
        }

        public abstract Task<LakossagData> LoadAsync();

        protected LakossagData Load(IExcelDataReader reader)
        {
            var ld = new LakossagData();

            ReadHeader(reader, ld);

            // next sheet
            reader.NextResult();

            ReadData(reader, ld);
            
            return ld;
        }

        protected virtual void ReadHeader(IExcelDataReader reader, LakossagData ld)
        {
            // the data is in the second row
            reader.Read();
            reader.Read();

            var dateText = reader.GetValue(1).ToString();

            if (DateTime.TryParse(dateText, out var date))
                ld.Date = date;
            else
                throw new InvalidOperationException("Query date not found.");
        }
        protected virtual void ReadData(IExcelDataReader reader, LakossagData ld)
        {
            var oevkDataAll = new List<OevkData>();
            var oevkDataItems = new List<OevkData>();

            // header
            reader.Read();

            while (reader.Read())
            {
                var od = new OevkData
                {
                    Name = reader.GetValue(0).ToString(),
                    Index = reader.GetValue(1).ToString()
                };

                for (var i = 3; i < reader.FieldCount; i++)
                {
                    var count = reader.GetValue(i);
                    od.Items.Add(Convert.ToInt32(count));
                }

                var town = reader.GetValue(2).ToString();
                if (string.Compare(town, "oevk összesen", StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    od.Town = town;
                    oevkDataAll.Add(od);

                    continue;
                }

                oevkDataItems.Add(od);

                //Console.WriteLine($"{od.Name} {od.Index}    " + string.Join(", ", od));
            }

            ld.AllItems = oevkDataAll;
            ld.OevkItems = oevkDataItems;
        }
    }
}
