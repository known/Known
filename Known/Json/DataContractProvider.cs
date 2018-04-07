using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Known.Json
{
    /// <summary>
    /// 数据契约JSON提供者。
    /// </summary>
    public class DataContractProvider : IJsonProvider
    {
        /// <summary>
        /// 将对象序列化成JSON字符串。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="value">对象实例。</param>
        /// <returns>JSON字符串。</returns>
        public string Serialize<T>(T value)
        {
            var settings = new DataContractJsonSerializerSettings
            {
                DateTimeFormat = new DateTimeFormat("yyyy-MM-dd HH:mm:ss")
            };
            var serializer = new DataContractJsonSerializer(typeof(T), settings);
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, value);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// 将JSON字符串反序列化成对象。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="json">JSON字符串。</param>
        /// <returns>对象实例。</returns>
        public T Deserialize<T>(string json)
        {
            var settings = new DataContractJsonSerializerSettings
            {
                DateTimeFormat = new DateTimeFormat("yyyy-MM-dd HH:mm:ss")
            };
            var serializer = new DataContractJsonSerializer(typeof(T), settings);
            var bytes = Encoding.Default.GetBytes(json);
            using (var stream = new MemoryStream(bytes))
            {
                return (T)serializer.ReadObject(stream);
            }
        }
    }
}
