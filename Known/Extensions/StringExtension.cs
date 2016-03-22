using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Known.Extensions
{
    public static class StringExtension
    {
        #region Validation
        public static bool IsIPAddress(this string value)
        {
            return Regex.IsMatch(value, @"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])((\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])){3}|(\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])){5})$");
        }

        public static bool IsEmail(this string value)
        {
            return Regex.IsMatch(value, @"^(\w)+(\.\w+)*@(\w)+((\.\w+)+)$");
        }
        #endregion

        public static string ToSql(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return value.Replace("'", "''");
        }

        public static string StripHtml(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            value = new Regex("<[^>]*>", RegexOptions.Compiled).Replace(value, string.Empty);
            return value.Trim();
        }

        public static string RemoveHtmlWhitespace(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            value = new Regex(@">\s+", RegexOptions.Compiled).Replace(value, ">");
            //value = new Regex(@"\n\s+", RegexOptions.Compiled).Replace(value, string.Empty);
            return value.Trim();
        }

        public static string ToMd5(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            var md5 = MD5.Create();
            var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(value));
            var sb = new StringBuilder();
            bytes.ForEach(b => sb.Append(b.ToString("x2")));
            return sb.ToString();
        }

        public static List<T> JsonToList<T>(this string json)
        {
            return json.FromJson<List<T>>();
        }

        public static T FromJson<T>(this string json)
        {
            if (string.IsNullOrEmpty(json))
                return default(T);

            var settings = new JsonSerializerSettings();
            settings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static T FromXml<T>(this string xml) where T : class
        {
            if (string.IsNullOrEmpty(xml))
                return default(T);

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(reader);
            }
        }
    }
}
