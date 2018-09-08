using System.Web;

namespace Known.Cache
{
    public class DefaultCache : ICache
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
}
