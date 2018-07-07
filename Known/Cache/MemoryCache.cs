using System;
using System.Runtime.Caching;

namespace Known.Cache
{
    public class MemoryCache : ICache
    {
        private readonly ObjectCache cache = System.Runtime.Caching.MemoryCache.Default;

        public int Count { get; }

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
}
