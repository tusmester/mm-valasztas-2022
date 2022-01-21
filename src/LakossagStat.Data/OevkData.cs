using System.Collections.Generic;
using System.Linq;

namespace LakossagStat.Data
{
    public class OevkData
    {
        public string Name { get; set; }
        public string Index { get; set; }
        public string Town { get; set; }
        public List<int> Items { get; } = new List<int>();

        //UNDONE: cache computed values if possible
        public int Difference => Items.Last() - Items.First();
        public double DifferencePercent => Difference * 100.0 / Items.First();
    }
}
