namespace Known.Json
{
    /// <summary>
    /// 默认JSON提供者类。
    /// </summary>
    public class DefaultJsonProvider : IJsonProvider
    {
        /// <summary>
        /// 将对象序列化成JSON字符串。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="value">对象实例。</param>
        /// <returns>JSON字符串。</returns>
        public string Serialize<T>(T value)
        {
            return new DataContractProvider().Serialize(value);
        }

        /// <summary>
        /// 将JSON字符串反序列化成对象。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="json">JSON字符串。</param>
        /// <returns>对象实例。</returns>
        public T Deserialize<T>(string json)
        {
            return new DataContractProvider().Deserialize<T>(json);
        }
    }
}
