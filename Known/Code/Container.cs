/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * ------------------------------------------------------------------------------- */

using System;
using System.Collections;
using System.Reflection;

namespace Known
{
    public sealed class Container
    {
        private static readonly Hashtable cached = new Hashtable();

        private Container() { }

        public static void Clear()
        {
            lock (cached.SyncRoot)
            {
                cached.Clear();
            }
        }

        public static T Resolve<T>(T objDefault = default)
        {
            var type = typeof(T);
            if (cached.ContainsKey(type))
                return (T)cached[type];

            var name = type.Name;
            if (cached.ContainsKey(name))
                return (T)cached[name];

            if (type.IsInterface)
            {
                var names = type.FullName.Split('.');
                for (int i = 0; i < names.Length; i++)
                {
                    if (i == names.Length - 1)
                    {
                        names[i] = names[i].Substring(1);
                    }
                }
                var implName = string.Join(".", names);
                var implType = type.Assembly.GetType(implName);
                if (implType != null)
                {
                    Register(type, () => Activator.CreateInstance(implType));
                    return (T)cached[type];
                }
            }

            if (objDefault != null)
            {
                Register(type, () => objDefault);
            }

            return objDefault;
        }

        public static void Register<T, TImpl>() where TImpl : T
        {
            Register(typeof(T), () => Activator.CreateInstance<TImpl>());
        }

        public static void Register<T>(T instance)
        {
            Register(typeof(T), () => instance);
        }

        public static object ResolveService(string name)
        {
            if (!cached.ContainsKey(name))
                return null;

            return cached[name];
        }

        public static void Register<TBase>(Assembly assembly)
        {
            foreach (var item in assembly.GetTypes())
            {
                if (item.IsSubclassOf(typeof(TBase)) && !item.IsAbstract)
                {
                    Register(item.Name, () => Activator.CreateInstance(item));
                }
            }
        }

        internal static void Register(object key, Func<object> func)
        {
            lock (cached.SyncRoot)
            {
                cached[key] = func();
            }
        }
    }
}