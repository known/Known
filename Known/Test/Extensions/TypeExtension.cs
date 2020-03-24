using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Known.Extensions
{
    /// <summary>
    /// 类型扩展类。
    /// </summary>
    public static class TypeExtension
    {
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> 
            TypeProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();

        /// <summary>
        /// 获取类型所有可读可写的属性集合。
        /// </summary>
        /// <param name="type">类型实例。</param>
        /// <returns>属性集合。</returns>
        public static List<PropertyInfo> GetTypeProperties(this Type type)
        {
            if (type == null)
                return null;

            if (TypeProperties.TryGetValue(type.TypeHandle, out IEnumerable<PropertyInfo> pis))
                return pis.ToList();

            var properties = type.GetProperties()
                                 .Where(p => p.CanRead && p.CanWrite)
                                 .ToArray();
            TypeProperties[type.TypeHandle] = properties;
            return properties.ToList();
        }

        /// <summary>
        /// 判断类型是否有指定名称的可读可写属性。
        /// </summary>
        /// <param name="type">类型实例。</param>
        /// <param name="propertyName">属性名称。</param>
        /// <returns>有返回 True，否则返回 False。</returns>
        public static bool HasTypeProperty(this Type type, string propertyName)
        {
            if (type == null)
                return false;

            var properties = type.GetTypeProperties();
            if (properties == null || properties.Count == 0)
                return false;

            return properties.Count(p => p.Name == propertyName) > 0;
        }

        /// <summary>
        /// 获取类型成员指定类型的特性对象。
        /// </summary>
        /// <typeparam name="T">特性类型。</typeparam>
        /// <param name="member">类型成员。</param>
        /// <param name="inherit">是否查找继承对象。</param>
        /// <returns>特性对象。</returns>
        public static T GetAttribute<T>(this MemberInfo member, bool inherit = true)
        {
            if (member == null)
                return default;

            foreach (var attr in member.GetCustomAttributes(inherit))
            {
                if (attr is T)
                {
                    return (T)attr;
                }
            }

            return default;
        }
    }
}
