using System;
using System.Globalization;

namespace Known.Extensions
{
    /// <summary>
    /// 日期时间扩展类。
    /// </summary>
    public static class DateTimeExtension
    {
        /// <summary>
        /// 获取日期时间值的时间戳。
        /// </summary>
        /// <param name="input">日期时间值。</param>
        /// <returns>时间戳。</returns>
        public static long ToTimestamp(this DateTime input)
        {
            var start = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return Convert.ToInt64((input - start).TotalMilliseconds);
        }

        /// <summary>
        /// 获取指定格式的日期时间值，若字符串与日期时间格式不匹配，返回空值。
        /// </summary>
        /// <param name="input">日期时间字符串。</param>
        /// <param name="format">日期时间格式。</param>
        /// <returns>可空的日期时间值，格式不正确，返回空。</returns>
        public static DateTime? ToDateTime(this string input, string format)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            if (DateTime.TryParseExact(input, format, null, DateTimeStyles.None, out DateTime result))
                return result;

            return null;
        }

        /// <summary>
        /// 获取指定格式的日期时间字符串。
        /// </summary>
        /// <param name="input">日期时间值。</param>
        /// <param name="format">日期时间格式。</param>
        /// <returns>日期时间字符串。</returns>
        public static string ToString(this DateTime? input, string format)
        {
            if (input.HasValue)
                return input.Value.ToString(format);

            return string.Empty;
        }
    }
}
