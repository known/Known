using System;
using System.Collections;

namespace Known
{
    /// <summary>
    /// 对象容器。
    /// </summary>
    public sealed class Container
    {
        private static Hashtable cached = new Hashtable();

        /// <summary>
        /// 注册对象。
        /// </summary>
        /// <typeparam name="T">对象接口类型。</typeparam>
        /// <typeparam name="TImpl">对象实现类型。</typeparam>
        public static void Register<T, TImpl>() where TImpl : T
        {
            var key = typeof(T);
            if (!cached.ContainsKey(key))
            {
                lock (cached.SyncRoot)
                {
                    cached[key] = Activator.CreateInstance<TImpl>();
                }
            }
        }

        /// <summary>
        /// 注册对象。
        /// </summary>
        /// <typeparam name="T">对象接口类型。</typeparam>
        /// <typeparam name="TImpl">对象实现类型。</typeparam>
        /// <param name="instance">对象实例。</param>
        public static void Register<T, TImpl>(TImpl instance) where TImpl : T
        {
            var key = typeof(T);
            if (!cached.ContainsKey(key))
            {
                lock (cached.SyncRoot)
                {
                    cached[key] = instance;
                }
            }
        }

        /// <summary>
        /// 加载注册的对象。
        /// </summary>
        /// <typeparam name="T">对象接口类型。</typeparam>
        /// <returns>对象实例。</returns>
        public static T Load<T>()
        {
            var key = typeof(T);
            if (cached.ContainsKey(key))
            {
                return (T)cached[key];
            }

            return default(T);
        }
    }
}
