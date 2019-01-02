using System;
using System.Collections;
using System.Reflection;

namespace Known
{
    public sealed class Container
    {
        private static Hashtable cached = new Hashtable();

        public static T Resolve<T>()
        {
            var key = typeof(T);
            if (cached.ContainsKey(key))
                return (T)cached[key];

            var key1 = typeof(T).Name;
            if (cached.ContainsKey(key1))
                return (T)cached[key1];

            return default(T);
        }

        public static object Resolve(string name)
        {
            if (!cached.ContainsKey(name))
                return null;

            return cached[name];
        }

        public static void Register<T, TImpl>() where TImpl : T
        {
            var key = typeof(T);
            if (!cached.ContainsKey(key))
            {
                lock (cached.SyncRoot)
                {
                    if (!cached.ContainsKey(key))
                    {
                        cached[key] = Activator.CreateInstance<TImpl>();
                    }
                }
            }
        }

        public static void Register<T, TImpl>(TImpl instance) where TImpl : T
        {
            var key = typeof(T);
            Register(key, instance);
        }

        public static void Register<T>(Assembly assembly, params object[] args)
        {
            if (assembly == null)
                return;

            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsSubclassOf(typeof(T)) && !type.IsAbstract)
                {
                    var instance = args != null && args.Length > 0
                                 ? Activator.CreateInstance(type, args)
                                 : Activator.CreateInstance(type);
                    Register(type.Name, instance);
                }
            }
        }

        private static void Register(object key, object instance)
        {
            if (!cached.ContainsKey(key))
            {
                lock (cached.SyncRoot)
                {
                    if (!cached.ContainsKey(key))
                    {
                        cached[key] = instance;
                    }
                }
            }
        }
    }
}
