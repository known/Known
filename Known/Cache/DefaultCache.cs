using System.Collections.Generic;

namespace Known.Cache
{
    public class DefaultCache : ICache
    {
        private static Dictionary<string, object> cached = new Dictionary<string, object>();

        public int Count
        {
            get { return cached.Count; }
        }

        public object Get(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            if (!cached.ContainsKey(key))
                return null;

            return cached[key];
        }

        public void Set(string key, object value)
        {
            if (value == null)
            {
                Remove(key);
                return;
            }

            cached[key] = value;
        }

        public void Set(string key, object value, int expires)
        {
            Set(key, value);
        }

        public void Remove(string key)
        {
            if (cached.ContainsKey(key))
            {
                cached.Remove(key);
            }
        }

        public void Clear()
        {
            cached.Clear();
        }
    }
}
