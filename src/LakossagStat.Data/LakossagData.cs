using System;
using System.Collections.Generic;

namespace LakossagStat.Data
{
    //public class DoubleOevkValue
    //{
    //    public OevkData Data { get; set; }
    //    public double Value { get; set; }
    //}
    //public class IntOevkValue
    //{
    //    public OevkData Data { get; set; }
    //    public int Value { get; set; }
    //}

    public class LakossagData
    {
        public DateTime Date { get; set; }
        public IEnumerable<OevkData> AllItems { get; set; }
        public IEnumerable<OevkData> OevkItems { get; set; }

        public OevkData MaxIncrease { get; set; }
        public OevkData MaxIncreasePercent { get; set; }
        public OevkData MaxDecrease { get; set; }
        public OevkData MaxDecreasePercent { get; set; }
    }
}
