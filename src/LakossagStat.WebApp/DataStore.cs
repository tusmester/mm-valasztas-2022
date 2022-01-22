using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LakossagStat.Data;

namespace LakossagStat.WebApp
{
    public static class DataStore
    {
        public static LakossagData LakossagData { get; set; } = new LakossagData();
        public static DateTime LastRefresh { get; set; } = DateTime.MinValue;

        public static bool IsEmpty => LakossagData?.MaxIncrease == null;
    }
}
