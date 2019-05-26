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
        /// <returns>JSON 字符串。</returns>
        string Serialize<T>(T value);

        /// <summary>
        /// 将 JSON 格式字符串反序列化成指定泛型类型的对象。
        /// </summary>
        /// <typeparam name="T">泛型类型。</typeparam>
        /// <param name="json">JSON 字符串。</param>
        /// <returns>泛型类型对象。</returns>
        T Deserialize<T>(string json);
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
        /// <returns>JSON 字符串。</returns>
        public string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// 将 JSON 格式字符串反序列化成指定泛型类型的对象。
        /// </summary>
        /// <typeparam name="T">泛型类型。</typeparam>
        /// <param name="json">JSON 字符串。</param>
        /// <returns>泛型类型对象。</returns>
        public T Deserialize<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return default;

            var settings = new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd HH:mm:ss"
            };
            return JsonConvert.DeserializeObject<T>(json, settings);
        }
    }

    class JsonDefault : IJson
    {
        public string Serialize<T>(T value)
        {
            var serializer = GetJsonSerializer<T>();
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, value);
                return Encoding.Default.GetString(stream.ToArray());
            }
        }

        public T Deserialize<T>(string json)
        {
            var serializer = GetJsonSerializer<T>();
            var bytes = Encoding.Default.GetBytes(json);
            using (var stream = new MemoryStream(bytes))
            {
                return (T)serializer.ReadObject(stream);
            }
        }

        private DataContractJsonSerializer GetJsonSerializer<T>()
        {
            var settings = new DataContractJsonSerializerSettings
            {
                DateTimeFormat = new DateTimeFormat("yyyy-MM-dd HH:mm:ss")
            };
            return new DataContractJsonSerializer(typeof(T), settings);
        }
    }
}
