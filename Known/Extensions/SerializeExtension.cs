using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Known.Extensions
{
    /// <summary>
    /// 对象序列化扩展类。
    /// </summary>
    public static class SerializeExtension
    {
        /// <summary>
        /// 将对象序列化成JSON格式字符串。
        /// </summary>
        /// <param name="value">对象。</param>
        /// <returns>JSON格式字符串。</returns>
        public static string ToJson(this object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// 将JSON格式字符串反序列化成指定类型的对象。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="json">JSON格式字符串。</param>
        /// <returns>指定类型对象。</returns>
        public static T FromJson<T>(this string json)
        {
            var settings = new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd HH:mm:ss"
            };
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        /// <summary>
        /// 将对象序列化成XML格式字符串。
        /// </summary>
        /// <param name="value">对象。</param>
        /// <returns>XML格式字符串。</returns>
        public static string ToXml(this object value)
        {
            if (value == null)
                return null;

            var settings = new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 };
            using (var stream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream, settings))
                {
                    var namespaces = new XmlSerializerNamespaces();
                    namespaces.Add("", "");
                    var serializer = new XmlSerializer(value.GetType());
                    serializer.Serialize(writer, value, namespaces);
                }
                var xml = Encoding.UTF8.GetString(stream.ToArray());
                //去除第一个字符问号
                return xml.Substring(1);
            }
        }

        /// <summary>
        /// 将XML格式字符串反序列化成指定类型的对象。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="json">XML格式字符串。</param>
        /// <returns>指定类型对象。</returns>
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
