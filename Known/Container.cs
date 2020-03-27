using System;
using System.Collections;

namespace Known
{
    public sealed class Container
    {
        private static readonly Hashtable cached = new Hashtable();

        public static void Clear()
        {
            cached.Clear();
        }

        public static T Resolve<T>()
        {
            var type = typeof(T);
            if (cached.ContainsKey(type))
                return (T)cached[type];

            var name = type.Name;
            if (cached.ContainsKey(name))
                return (T)cached[name];

            return default;
        }

        public static void Register<T, TImpl>() where TImpl : T
        {
            Register(typeof(T), () => Activator.CreateInstance<TImpl>());
        }

        private static void Register(object key, Func<object> func)
        {
            lock (cached.SyncRoot)
            {
                cached[key] = func();
            }
        }
    }
}
