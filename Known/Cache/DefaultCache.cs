using System.Collections.Generic;

namespace Known.Cache
{
    /// <summary>
    /// 默认缓存类，使用应用程序内存进行缓存。
    /// </summary>
    public class DefaultCache : ICache
    {
        private static Dictionary<string, object> cached = new Dictionary<string, object>();

        /// <summary>
        /// 根据key获取缓存对象。
        /// </summary>
        /// <param name="key">缓存对象key。</param>
        /// <returns>缓存对象。</returns>
        public object Get(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            if (!cached.ContainsKey(key))
                return null;

            return cached[key];
        }

        /// <summary>
        /// 设置缓存对象。
        /// </summary>
        /// <param name="key">缓存对象key。</param>
        /// <param name="value">缓存对象。</param>
        public void Set(string key, object value)
        {
            if (value == null)
            {
                Remove(key);
                return;
            }

            cached[key] = value;
        }

        /// <summary>
        /// 设置缓存对象，并指定过期时间。
        /// </summary>
        /// <param name="key">缓存对象key。</param>
        /// <param name="value">缓存对象。</param>
        /// <param name="expires">过期时间，单位分钟。</param>
        public void Set(string key, object value, int expires)
        {
            Set(key, value);
        }

        /// <summary>
        /// 根据key移除缓存对象。
        /// </summary>
        /// <param name="key">缓存对象key。</param>
        public void Remove(string key)
        {
            if (cached.ContainsKey(key))
            {
                cached.Remove(key);
            }
        }

        /// <summary>
        /// 移除所有缓存对象。
        /// </summary>
        public void RemoveAll()
        {
            cached.Clear();
        }
    }
}
