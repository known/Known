using System;
using System.Collections;

namespace Known
{
    public interface IAppContext
    {
        void Clear();
        T GetValue<T>(string key);
        void SetValue<T>(string key, T value);
    }

    public abstract class AppContext : IAppContext
    {
        public abstract void Clear();
        public abstract T GetValue<T>(string key);
        public abstract void SetValue<T>(string key, T value);

        public static T LoadService<T>()
        {
            return ServiceLoader.Load<T>();
        }

        public static void RegisterService<T, TService>() where TService : T
        {
            ServiceLoader.Register<T, TService>();
        }

        static class ServiceLoader
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
}
