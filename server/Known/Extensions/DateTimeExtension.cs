using System;
using System.Globalization;

namespace Known.Extensions
{
    public static class DateTimeExtension
    {
        public static long ToTimestamp(this DateTime input)
        {
            var start = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return Convert.ToInt64((input - start).TotalMilliseconds);
        }

        public static DateTime? ToDateTime(this string input, string format)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            if (DateTime.TryParseExact(input, format, null, DateTimeStyles.None, out DateTime result))
                return result;

            return null;
        }

        public static string ToString(this DateTime? input, string format)
        {
            if (input.HasValue)
                return input.Value.ToString(format);

            return string.Empty;
        }
    }
}
