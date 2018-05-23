using Known.Serialization;
using Newtonsoft.Json;

namespace Known.Web.Providers
{
    /// <summary>
    /// JSON提供者。
    /// </summary>
    public class JsonProvider : IJsonProvider
    {
        /// <summary>
        /// 将对象序列化成JSON字符串。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="value">对象实例。</param>
        /// <returns>JSON字符串。</returns>
        public string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// 将JSON字符串反序列化成对象。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="json">JSON字符串。</param>
        /// <returns>对象实例。</returns>
        public T Deserialize<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return default(T);

            try
            {
                var settings = new JsonSerializerSettings
                {
                    DateFormatString = "yyyy-MM-dd HH:mm:ss"
                };
                return JsonConvert.DeserializeObject<T>(json, settings);
            }
            catch
            {
                return default(T);
            }
        }
    }
}