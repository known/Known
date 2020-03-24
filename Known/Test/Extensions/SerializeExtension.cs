using System;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
        /// 获取指定类型对象的 JSON 字符串。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="value">对象。</param>
        /// <param name="dateFormat">日期时间格式。</param>
        /// <returns>JSON 字符串。</returns>
        public static string ToJson<T>(this T value, string dateFormat = "yyyy-MM-dd HH:mm:ss")
        {
            if (value == null)
                return string.Empty;

            var provider = Container.Resolve<IJson>();
            if (provider == null)
                provider = new JsonProvider();

            return provider.Serialize(value, dateFormat);
        }

        /// <summary>
        /// 获取 JSON 反序列化后的指定类型的对象。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="json">JSON 字符串。</param>
        /// <param name="dateFormat">日期时间格式。</param>
        /// <returns>反序列化后的对象。</returns>
        public static T FromJson<T>(this string json, string dateFormat = "yyyy-MM-dd HH:mm:ss")
        {
            if (string.IsNullOrWhiteSpace(json))
                return default;

            var provider = Container.Resolve<IJson>();
            if (provider == null)
                provider = new JsonProvider();

            return provider.Deserialize<T>(json, dateFormat);
        }

        /// <summary>
        /// 获取 JSON 反序列化后的指定类型的对象。
        /// </summary>
        /// <param name="json">JSON 字符串。</param>
        /// <param name="type">对象类型。</param>
        /// <param name="dateFormat">日期时间格式。</param>
        /// <returns>反序列化后的对象。</returns>
        public static object FromJson(this string json, Type type, string dateFormat = "yyyy-MM-dd HH:mm:ss")
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            var provider = Container.Resolve<IJson>();
            if (provider == null)
                provider = new JsonProvider();

            return provider.Deserialize(json, type, dateFormat);
        }

        /// <summary>
        /// 获取指定对象的 XML 字符串。
        /// </summary>
        /// <param name="value">对象。</param>
        /// <returns>XML 字符串。</returns>
        public static string ToXml(this object value)
        {
            if (value == null)
                return string.Empty;

            if (value is DataTable)
            {
                var sb = new StringBuilder();
                var writer = XmlWriter.Create(sb);
                var serializer = new XmlSerializer(typeof(DataTable));
                serializer.Serialize(writer, value);
                writer.Close();
                return sb.ToString();
            }

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
                //remove character ?
                return xml.Substring(1);
            }
        }

        /// <summary>
        /// 获取 XML 反序列化后的指定类型的对象。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="xml">XML 字符串。</param>
        /// <returns>反序列化后的对象。</returns>
        public static T FromXml<T>(this string xml) where T : class
        {
            if (string.IsNullOrWhiteSpace(xml))
                return default;

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// 获取指定对象的序列化字节。
        /// </summary>
        /// <param name="value">对象。</param>
        /// <returns>对象的序列化字节。</returns>
        public static byte[] ToBytes(this object value)
        {
            if (value == null)
                return null;

            byte[] bytes = null;
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, value);
                bytes = ms.ToArray();
                ms.Flush();
                ms.Close();
            }
            return bytes;
        }

        /// <summary>
        /// 获取字节反序列化后的对象。
        /// </summary>
        /// <param name="buffer">反序列化字节。</param>
        /// <returns>反序列化后的对象。</returns>
        public static object FromBytes(this byte[] buffer)
        {
            if (buffer == null)
                return null;

            using (var ms = new MemoryStream(buffer))
            {
                var bf = new BinaryFormatter();
                return bf.Deserialize(ms);
            }
        }
    }
}
