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

        public static T Get<T>(string key)
        {
            return Cached.Get<T>(key);
        }

        public static void Set(string key, object value)
        {
            Cached.Set(key, value);
        }

        public static void Set(string key, object value, int expires)
        {
            Cached.Set(key, value, expires);
        }

        public static void Remove(string key)
        {
            Cache.Remove(key);
        }

        public static void Clear()
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
        private readonly ObjectCache cache = System.Runtime.Caching.MemoryCache.Default;

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
