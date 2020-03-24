using System.Text;
using System.Xml;

namespace Known.Extensions
{
    /// <summary>
    /// 字符串扩展类。
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 获取字符串字节长度。
        /// </summary>
        /// <param name="value">字符串。</param>
        /// <returns>字符串字节长度。</returns>
        public static int ByteLength(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            return Encoding.Default.GetBytes(value).Length;
        }

        /// <summary>
        /// 根据字节位置截取子字符串。
        /// </summary>
        /// <param name="value">字符串。</param>
        /// <param name="startIndex">开始位置。</param>
        /// <returns>截取的子字符串。</returns>
        public static string ByteSubstring(this string value, int startIndex)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var length = value.ByteLength();
            return value.ByteSubstring(startIndex, length);
        }

        /// <summary>
        /// 根据字节位置截取指定长度的子字符串。
        /// </summary>
        /// <param name="value">字符串。</param>
        /// <param name="startIndex">开始位置。</param>
        /// <param name="length">截取长度。</param>
        /// <returns>截取的子字符串。</returns>
        public static string ByteSubstring(this string value, int startIndex, int length)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var actualLength = value.ByteLength();
            var count = length > (actualLength - startIndex)
                      ? actualLength - startIndex
                      : length;
            var encoding = Encoding.GetEncoding("GB2312");
            var bytes = encoding.GetBytes(value);
            return encoding.GetString(bytes, startIndex, count);
        }

        /// <summary>
        /// 获取指定开始和结束字符串之间的字符串。
        /// </summary>
        /// <param name="text">源字符串。</param>
        /// <param name="start">开始字符串。</param>
        /// <param name="end">结束字符串。</param>
        /// <returns>子字符串。</returns>
        public static string SubString(this string text, string start, string end)
        {
            var startIndex = text.IndexOf(start);
            if (startIndex < 0)
                return string.Empty;

            var endIndex = text.IndexOf(end, startIndex);
            return text.Substring(startIndex, endIndex - startIndex).Replace(start, "");
        }

        /// <summary>
        /// 获取缩进等格式化的 XML 字符串。
        /// </summary>
        /// <param name="xml">XML 字符串。</param>
        /// <returns>格式化的 XML 字符串。</returns>
        public static string FormatXml(this string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
                return string.Empty;

            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var sb = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.UTF8
            };
            using (var xtw = XmlWriter.Create(sb, settings))
            {
                doc.WriteTo(xtw);
            }
            return sb.ToString();
        }
    }
}
