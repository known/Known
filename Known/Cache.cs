using System;
using System.Runtime.Caching;
using System.Web;

namespace Known
{
    public interface ICache
    {
        int Count { get; }

        T Get<T>(string key);
        object Get(string key);
        void Set(string key, object value);
        void Set(string key, object value, int expires);
        void Remove(string key);
        void Clear();
    }

    internal class DefaultCache : ICache
    {
        private ICache cache = null;

        public DefaultCache()
        {
            if (HttpContext.Current != null)
                cache = new WebCache();
            else
                cache = new MemoryCache();
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

    internal class MemoryCache : ICache
    {
        private readonly ObjectCache cache = System.Runtime.Caching.MemoryCache.Default;

        public int Count
        {
            get { return (int)cache.GetCount(); }
        }

        public T Get<T>(string key)
        {
            var value = Get(key);
            if (value == null)
                return default(T);

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

    internal class WebCache : ICache
    {
        public int Count
        {
            get { return HttpRuntime.Cache.Count; }
        }

        public T Get<T>(string key)
        {
            var value = Get(key);
            if (value == null)
                return default(T);

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
