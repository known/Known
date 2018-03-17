using System;
using System.Collections;

namespace Known
{
    /// <summary>
    /// 业务逻辑工厂类。
    /// </summary>
    public class BusinessFactory
    {
        private static Hashtable cached = new Hashtable();

        /// <summary>
        /// 创建业务逻辑对象。
        /// </summary>
        /// <typeparam name="T">业务逻辑对象类型。</typeparam>
        /// <param name="context">上下文对象。</param>
        /// <returns>业务逻辑实例。</returns>
        public static T Create<T>(Context context) where T : Business
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
