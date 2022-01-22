using System;
using TimeZoneInfo = System.TimeZoneInfo;

namespace LakossagStat.WebApp
{
    public static class DateConverters
    {
        private static readonly TimeZoneInfo TziHu = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

        public static DateTime ToCentralEuropean(this DateTime utcDate)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcDate, TziHu);
        }
    }
}
