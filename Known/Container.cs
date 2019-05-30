using System;
using System.Collections;
using System.Reflection;

namespace Known
{
    /// <summary>
    /// 应用程序对象容器类。
    /// </summary>
    public sealed class Container
    {
        private static readonly Hashtable cached = new Hashtable();

        /// <summary>
        /// 清除所有缓存的对象实例。
        /// </summary>
        public static void Clear()
        {
            cached.Clear();
        }

        /// <summary>
        /// 移除缓存中指定泛型类型的对象实例。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        public static void Remove<T>()
        {
            var key = typeof(T);
            if (cached.ContainsKey(key))
            {
                cached.Remove(key);
                return;
            }

            var key1 = typeof(T).Name;
            if (cached.ContainsKey(key1))
                cached.Remove(key1);
        }

        /// <summary>
        /// 移除缓存中指定类型名称的对象实例。
        /// </summary>
        /// <param name="name">对象类型名称。</param>
        public static void Remove(string name)
        {
            if (cached.ContainsKey(name))
            {
                cached.Remove(name);
            }
        }

        /// <summary>
        /// 获取指定泛型类型的对象实例。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <returns>对象实例。</returns>
        public static T Resolve<T>()
        {
            var key = typeof(T);
            if (cached.ContainsKey(key))
                return (T)cached[key];

            var key1 = typeof(T).Name;
            if (cached.ContainsKey(key1))
                return (T)cached[key1];

            return default;
        }

        /// <summary>
        /// 获取指定类型名称的对象实例。
        /// </summary>
        /// <param name="name">对象类型名称。</param>
        /// <returns>对象实例。</returns>
        public static object Resolve(string name)
        {
            if (!cached.ContainsKey(name))
                return null;

            return cached[name];
        }

        /// <summary>
        /// 注册指定泛型类型的对象，自动创建无参数构造函数的类型实例。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <typeparam name="TImpl">无参数构造函数的对象实例类型，必须继承 T。</typeparam>
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

        /// <summary>
        /// 注册指定泛型类型实例的对象。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="instance">对象实例。</param>
        public static void Register<T>(object instance)
        {
            var key = typeof(T);
            Register(key, instance);
        }

        /// <summary>
        /// 注册程序集中所有实现 T 的类型对象实例。
        /// </summary>
        /// <typeparam name="T">对象基类类型。</typeparam>
        /// <param name="assembly">程序集。</param>
        /// <param name="args">构造函数参数。</param>
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
