using System;
using System.Collections;

namespace Known
{
    public class BusinessFactory
    {
        private static Hashtable cached = new Hashtable();

        public static T Create<T>(Context context) where T : BusinessBase
        {
            var key = typeof(T);
            if (!cached.ContainsKey(key))
            {
                lock (cached.SyncRoot)
                {
                    if (!cached.ContainsKey(key))
                    {
                        cached[key] = Activator.CreateInstance(typeof(T), context);
                    }
                }
            }

            return (T)cached[key];
        }
    }
}
