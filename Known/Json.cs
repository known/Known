using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Newtonsoft.Json;

namespace Known
{
    /// <summary>
    /// JSON 序列化接口。
    /// </summary>
    public interface IJson
    {
        /// <summary>
        /// 将泛型对象序列化成 JSON 格式。
        /// </summary>
        /// <typeparam name="T">泛型类型。</typeparam>
        /// <param name="value">泛型类型对象。</param>
        /// <param name="dateFormat">日期时间格式。</param>
        /// <returns>JSON 字符串。</returns>
        string Serialize<T>(T value, string dateFormat = "yyyy-MM-dd HH:mm:ss");

        /// <summary>
        /// 将 JSON 格式字符串反序列化成指定泛型类型的对象。
        /// </summary>
        /// <typeparam name="T">泛型类型。</typeparam>
        /// <param name="json">JSON 字符串。</param>
        /// <param name="dateFormat">日期时间格式。</param>
        /// <returns>泛型类型对象。</returns>
        T Deserialize<T>(string json, string dateFormat = "yyyy-MM-dd HH:mm:ss");

        /// <summary>
        /// 将 JSON 格式字符串反序列化成指定类型的对象。
        /// </summary>
        /// <param name="json">JSON 字符串。</param>
        /// <param name="type">反序列化的类型。</param>
        /// <param name="dateFormat">日期时间格式。</param>
        /// <returns>泛型类型对象。</returns>
        object Deserialize(string json, Type type, string dateFormat = "yyyy-MM-dd HH:mm:ss");
    }

    /// <summary>
    /// Newtonsoft.Json 序列化提供者。
    /// </summary>
    public class JsonProvider : IJson
    {
        /// <summary>
        /// 将泛型对象序列化成 JSON 格式。
        /// </summary>
        /// <typeparam name="T">泛型类型。</typeparam>
        /// <param name="value">泛型类型对象。</param>
        /// <param name="dateFormat">日期时间格式。</param>
        /// <returns>JSON 字符串。</returns>
        public string Serialize<T>(T value, string dateFormat = "yyyy-MM-dd HH:mm:ss")
        {
            if (value == null)
                return string.Empty;

            var settings = new JsonSerializerSettings { DateFormatString = dateFormat };
            return JsonConvert.SerializeObject(value, settings);
        }

        /// <summary>
        /// 将 JSON 格式字符串反序列化成指定泛型类型的对象。
        /// </summary>
        /// <typeparam name="T">泛型类型。</typeparam>
        /// <param name="json">JSON 字符串。</param>
        /// <param name="dateFormat">日期时间格式。</param>
        /// <returns>泛型类型对象。</returns>
        public T Deserialize<T>(string json, string dateFormat = "yyyy-MM-dd HH:mm:ss")
        {
            if (string.IsNullOrWhiteSpace(json))
                return default;

            var settings = new JsonSerializerSettings { DateFormatString = dateFormat };
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        /// <summary>
        /// 将 JSON 格式字符串反序列化成指定类型的对象。
        /// </summary>
        /// <param name="json">JSON 字符串。</param>
        /// <param name="type">反序列化的类型。</param>
        /// <param name="dateFormat">日期时间格式。</param>
        /// <returns>泛型类型对象。</returns>
        public object Deserialize(string json, Type type, string dateFormat = "yyyy-MM-dd HH:mm:ss")
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            var settings = new JsonSerializerSettings { DateFormatString = dateFormat };
            return JsonConvert.DeserializeObject(json, type, settings);
        }
    }

    class JsonDefault : IJson
    {
        public string Serialize<T>(T value, string dateFormat = "yyyy-MM-dd HH:mm:ss")
        {
            if (value == null)
                return string.Empty;

            var serializer = GetJsonSerializer(typeof(T), dateFormat);
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, value);
                return Encoding.Default.GetString(stream.ToArray());
            }
        }

        public T Deserialize<T>(string json, string dateFormat = "yyyy-MM-dd HH:mm:ss")
        {
            if (string.IsNullOrWhiteSpace(json))
                return default;

            var serializer = GetJsonSerializer(typeof(T), dateFormat);
            var bytes = Encoding.Default.GetBytes(json);
            using (var stream = new MemoryStream(bytes))
            {
                return (T)serializer.ReadObject(stream);
            }
        }

        public object Deserialize(string json, Type type, string dateFormat = "yyyy-MM-dd HH:mm:ss")
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            var serializer = GetJsonSerializer(type, dateFormat);
            var bytes = Encoding.Default.GetBytes(json);
            using (var stream = new MemoryStream(bytes))
            {
                return serializer.ReadObject(stream);
            }
        }

        private DataContractJsonSerializer GetJsonSerializer(Type type, string dateFormat)
        {
            var settings = new DataContractJsonSerializerSettings
            {
                DateTimeFormat = new DateTimeFormat(dateFormat)
            };
            return new DataContractJsonSerializer(type, settings);
        }
    }
}
