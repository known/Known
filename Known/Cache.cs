using System;
using System.Runtime.Caching;
using System.Web;

namespace Known
{
    /// <summary>
    /// 应用程序缓存接口。
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// 取得缓存对象总数。
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 获取指定关键字的泛型缓存对象。
        /// </summary>
        /// <typeparam name="T">缓存对象泛型。</typeparam>
        /// <param name="key">缓存对象关键字。</param>
        /// <returns>泛型缓存对象。</returns>
        T Get<T>(string key);

        /// <summary>
        /// 获取指定关键字的缓存对象。
        /// </summary>
        /// <param name="key">缓存对象关键字。</param>
        /// <returns>缓存对象。</returns>
        object Get(string key);

        /// <summary>
        /// 设置缓存对象。
        /// </summary>
        /// <param name="key">缓存对象关键字。</param>
        /// <param name="value">缓存对象。</param>
        void Set(string key, object value);

        /// <summary>
        /// 设置缓存对象，并添加过期时间（分钟）。
        /// </summary>
        /// <param name="key">缓存对象关键字。</param>
        /// <param name="value">缓存对象。</param>
        /// <param name="expires">过期分钟数。</param>
        void Set(string key, object value, int expires);

        /// <summary>
        /// 移除指定关键字的缓存对象。
        /// </summary>
        /// <param name="key">缓存对象关键字。</param>
        void Remove(string key);

        /// <summary>
        /// 清除所有缓存对象。
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// 应用程序缓存，默认根据当前程序上下文环境初始化缓存接口。
    /// 支持 HttpRuntime.Cache 和 System.Runtime.Caching.MemoryCache。
    /// </summary>
    public sealed class Cache
    {
        private static ICache Cached
        {
            get
            {
                var cache = Container.Resolve<ICache>();
                if (cache == null)
                    cache = new CacheDefault();

                return cache;
            }
        }

        /// <summary>
        /// 取得缓存对象总数。
        /// </summary>
        public int Count
        {
            get { return Cached.Count; }
        }

        /// <summary>
        /// 获取指定关键字的泛型缓存对象。
        /// </summary>
        /// <typeparam name="T">缓存对象泛型。</typeparam>
        /// <param name="key">缓存对象关键字。</param>
        /// <returns>泛型缓存对象。</returns>
        public T Get<T>(string key)
        {
            return Cached.Get<T>(key);
        }

        /// <summary>
        /// 获取指定关键字的缓存对象。
        /// </summary>
        /// <param name="key">缓存对象关键字。</param>
        /// <returns>缓存对象。</returns>
        public object Get(string key)
        {
            return Cached.Get(key);
        }

        /// <summary>
        /// 设置缓存对象。
        /// </summary>
        /// <param name="key">缓存对象关键字。</param>
        /// <param name="value">缓存对象。</param>
        public void Set(string key, object value)
        {
            Cached.Set(key, value);
        }

        /// <summary>
        /// 设置缓存对象，并添加过期时间（分钟）。
        /// </summary>
        /// <param name="key">缓存对象关键字。</param>
        /// <param name="value">缓存对象。</param>
        /// <param name="expires">过期分钟数。</param>
        public void Set(string key, object value, int expires)
        {
            Cached.Set(key, value, expires);
        }

        /// <summary>
        /// 移除指定关键字的缓存对象。
        /// </summary>
        /// <param name="key">缓存对象关键字。</param>
        public void Remove(string key)
        {
            Cached.Remove(key);
        }

        /// <summary>
        /// 清除所有缓存对象。
        /// </summary>
        public void Clear()
        {
            Cached.Clear();
        }
    }

    class CacheDefault : ICache
    {
        private readonly ICache cache = null;

        public CacheDefault()
        {
            if (HttpContext.Current != null)
                cache = new CacheWeb();
            else
                cache = new CacheMemory();
        }

        public int Count
        {
            get { return cache.Count; }
        }

        public T Get<T>(string key)
        {
            return cache.Get<T>(key);
        }

        public object Get(string key)
        {
            return cache.Get(key);
        }

        public void Set(string key, object value)
        {
            cache.Set(key, value);
        }

        public void Set(string key, object value, int expires)
        {
            cache.Set(key, value, expires);
        }

        public void Remove(string key)
        {
            cache.Remove(key);
        }

        public void Clear()
        {
            cache.Clear();
        }
    }

    class CacheMemory : ICache
    {
        private readonly ObjectCache cache = MemoryCache.Default;

        public int Count
        {
            get { return (int)cache.GetCount(); }
        }

        public T Get<T>(string key)
        {
            var value = Get(key);
            if (value == null)
                return default;

            return (T)value;
        }

        public object Get(string key)
        {
            return cache[key];
        }

        public void Set(string key, object value)
        {
            if (value == null)
            {
                Remove(key);
                return;
            }

            cache.Set(new CacheItem(key, value), new CacheItemPolicy());
        }

        public void Set(string key, object value, int expires)
        {
            if (value == null)
            {
                Remove(key);
                return;
            }

            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(expires)
            };
            cache.Add(new CacheItem(key, value), policy);
        }

        public void Remove(string key)
        {
            cache.Remove(key);
        }

        public void Clear()
        {
            foreach (var item in cache)
            {
                Remove(item.Key);
            }
        }
    }

    class CacheWeb : ICache
    {
        public int Count
        {
            get { return HttpRuntime.Cache.Count; }
        }

        public T Get<T>(string key)
        {
            var value = Get(key);
            if (value == null)
                return default;

            return (T)value;
        }

        public object Get(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            return HttpRuntime.Cache.Get(key);
        }

        public void Set(string key, object value)
        {
            if (value == null)
            {
                Remove(key);
                return;
            }

            HttpRuntime.Cache.Insert(key, value);
        }

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

        public void Remove(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }

        public void Clear()
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
