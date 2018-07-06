using System;
using System.Web;

namespace Known.Cache
{
    public class WebCache : ICache
    {
        public int Count
        {
            get { return HttpRuntime.Cache.Count; }
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
