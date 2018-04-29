using System;
using System.Web;

namespace Known.Cache
{
    /// <summary>
    /// Web应用程序缓存。
    /// </summary>
    public class WebCache : ICache
    {
        /// <summary>
        /// 取得缓存项目数量。
        /// </summary>
        public int Count
        {
            get { return HttpRuntime.Cache.Count; }
        }

        /// <summary>
        /// 根据key获取缓存对象。
        /// </summary>
        /// <param name="key">缓存对象key。</param>
        /// <returns>缓存对象。</returns>
        public object Get(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            return HttpRuntime.Cache.Get(key);
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

            HttpRuntime.Cache.Insert(key, value);
        }

        /// <summary>
        /// 设置缓存对象，并指定过期时间。
        /// </summary>
        /// <param name="key">缓存对象key。</param>
        /// <param name="value">缓存对象。</param>
        /// <param name="expires">过期时间，单位分钟。</param>
        public void Set(string key, object value, int expires)
        {
            if (value == null)
            {
                Remove(key);
                return;
            }

            HttpRuntime.Cache.Insert(key, value, null, 
                System.Web.Caching.Cache.NoAbsoluteExpiration,
                new TimeSpan(0, expires, 0)
            );
        }

        /// <summary>
        /// 根据key移除缓存对象。
        /// </summary>
        /// <param name="key">缓存对象key。</param>
        public void Remove(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }

        /// <summary>
        /// 移除所有缓存对象。
        /// </summary>
        public void RemoveAll()
        {
            var cache = HttpRuntime.Cache;
            var cacheEnum = cache.GetEnumerator();
            while (cacheEnum.MoveNext())
            {
                cache.Remove(cacheEnum.Key.ToString());
            }
        }
    }
}
