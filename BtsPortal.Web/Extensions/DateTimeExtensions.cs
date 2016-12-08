using System;
using BtsPortal.Web.Infrastructure.Settings;

namespace BtsPortal.Web.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime? ToUtcDateTime(this DateTime? localDateTime)
        {
            if (localDateTime == null)
            {
                return null;
            }

            return TimeZoneInfo.ConvertTimeToUtc(localDateTime.Value, TimeZoneInfo.Local);
        }

        public static DateTime? ToLocalDateTime(this DateTime? utcDateTime)
        {
            if (utcDateTime == null)
            {
                return null;
            }

            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime.Value, TimeZoneInfo.Local);
        }

        public static string ToDisplayedDateTime(this string dbDateTime)
        {
            if (string.IsNullOrWhiteSpace(dbDateTime))
            {
                return "";
            }

            return Convert.ToDateTime(dbDateTime).ToString(AppSettings.DateTimeDisplay);
        }

        public static string ToDisplayDateTime(this DateTime? dbDateTime)
        {
            if (!dbDateTime.HasValue)
            {
                return String.Empty;
            }

            return dbDateTime.Value.ToString(AppSettings.DateTimeDisplay);
        }
    }
}