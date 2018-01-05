namespace Known.Cache
{
    /// <summary>
    /// 缓存接口。
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// 根据key获取缓存对象。
        /// </summary>
        /// <param name="key">缓存对象key。</param>
        /// <returns>缓存对象。</returns>
        object Get(string key);

        /// <summary>
        /// 设置缓存对象。
        /// </summary>
        /// <param name="key">缓存对象key。</param>
        /// <param name="value">缓存对象。</param>
        void Set(string key, object value);

        /// <summary>
        /// 设置缓存对象，并指定过期时间。
        /// </summary>
        /// <param name="key">缓存对象key。</param>
        /// <param name="value">缓存对象。</param>
        /// <param name="expires">过期时间，单位秒。</param>
        void Set(string key, object value, int expires);

        /// <summary>
        /// 根据key移除缓存对象。
        /// </summary>
        /// <param name="key">缓存对象key。</param>
        void Remove(string key);

        /// <summary>
        /// 移除所有缓存对象。
        /// </summary>
        void RemoveAll();
    }
}
