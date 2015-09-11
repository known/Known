using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Known
{
    public static class ServiceLoader
    {
        private static Hashtable cachedServices = new Hashtable();

        public static void Register<T, TService>() where TService : T
        {
            var key = typeof(T);
            if (!cachedServices.ContainsKey(key))
            {
                lock (cachedServices.SyncRoot)
                {
                    cachedServices[key] = Activator.CreateInstance<TService>();
                }
            }
        }

        public static T Load<T>()
        {
            var key = typeof(T);
            if (cachedServices.ContainsKey(key))
            {
                return (T)cachedServices[key];
            }

            return default(T);
        }
    }
}
