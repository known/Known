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
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> TypeProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();

        /// <summary>
        /// 获取类型的所有数据实体表栏位属性对象集合，默认约定可读写且非虚拟的Public属性为数据实体表栏位属性。
        /// </summary>
        /// <param name="type">实体类型。</param>
        /// <returns>所有数据实体表栏位属性对象集合。</returns>
        public static List<PropertyInfo> GetColumnProperties(this Type type)
        {
            if (type == null)
                return null;

            if (TypeProperties.TryGetValue(type.TypeHandle, out IEnumerable<PropertyInfo> pis))
                return pis.ToList();

            var properties = type.GetProperties()
                                 .Where(p => p.CanRead && p.CanWrite && !(p.SetMethod.IsVirtual && !p.SetMethod.IsFinal))
                                 .ToArray();
            TypeProperties[type.TypeHandle] = properties;
            return properties.ToList();
        }

        /// <summary>
        /// 判断类型是否有指定属性名的数据实体表栏位属性。
        /// </summary>
        /// <param name="type">类型。</param>
        /// <param name="propertyName">指定属性名。</param>
        /// <returns>有属性返回true，否则返回false。</returns>
        public static bool HasColumnProperty(this Type type, string propertyName)
        {
            if (type == null)
                return false;

            var properties = type.GetColumnProperties();
            if (properties == null || properties.Count == 0)
                return false;

            return properties.Count(p => p.Name == propertyName) > 0;
        }

        /// <summary>
        /// 获取类型成员的指定类型的特性。
        /// </summary>
        /// <typeparam name="T">特性类型。</typeparam>
        /// <param name="member">类型成员。</param>
        /// <param name="inherit">是否继承查找。</param>
        /// <returns>指定类型的特性。</returns>
        public static T GetAttribute<T>(this MemberInfo member, bool inherit = true)
        {
            if (member == null)
                return default(T);

            foreach (var attr in member.GetCustomAttributes(inherit))
            {
                if (attr is T)
                {
                    return (T)attr;
                }
            }

            return default(T);
        }
    }
}
