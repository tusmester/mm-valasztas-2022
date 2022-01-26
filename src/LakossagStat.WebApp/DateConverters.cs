using System;
using System.Runtime.InteropServices;
using TimeZoneInfo = System.TimeZoneInfo;

namespace LakossagStat.WebApp
{
    public static class DateConverters
    {
        private static readonly TimeZoneInfo TziHu =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time")
                : TimeZoneInfo.FindSystemTimeZoneById("Europe/Budapest");

        public static DateTime ToCentralEuropean(this DateTime utcDate)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcDate, TziHu);
        }
    }
}
