using System;
using System.Globalization;

namespace Known.Extensions
{
    /// <summary>
    /// 日期扩展类。
    /// </summary>
    public static class DateTimeExtension
    {
        /// <summary>
        /// 获取日期的时间戳。
        /// </summary>
        /// <param name="input">日期值。</param>
        /// <returns>日期的时间戳。</returns>
        public static long ToTimestamp(this DateTime input)
        {
            var start = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return Convert.ToInt64((input - start).TotalMilliseconds);
        }

        /// <summary>
        /// 根据格式将字符串转换成可空日期。
        /// </summary>
        /// <param name="input">日期字符串。</param>
        /// <param name="format">日期格式。</param>
        /// <returns></returns>
        public static DateTime? ToDateTime(this string input, string format)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            if (DateTime.TryParseExact(input, format, null, DateTimeStyles.None, out DateTime result))
                return result;

            return null;
        }

        /// <summary>
        /// 根据格式将可空日期转换成字符串。
        /// </summary>
        /// <param name="input">日期值。</param>
        /// <param name="format">日期格式。</param>
        /// <returns>格式化后的日期字符串。</returns>
        public static string ToString(this DateTime? input, string format)
        {
            if (input.HasValue)
                return input.Value.ToString(format);

            return string.Empty;
        }
    }
}
