using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Known.Extensions
{
    public static class TypeExtension
    {
        private static Hashtable cachedProperties = new Hashtable();
        public static List<PropertyInfo> GetProperties(this Type type)
        {
            if (type == null)
                return null;

            if (cachedProperties.ContainsKey(type))
            {
                return cachedProperties[type] as List<PropertyInfo>;
            }

            var properties = type.GetProperties().ToList();
            lock (cachedProperties.SyncRoot)
            {
                cachedProperties[type] = properties;
            }

            return properties;
        }

        public static T GetAttribute<T>(this MemberInfo member)
        {
            return member.GetAttribute<T>(true);
        }

        public static T GetAttribute<T>(this MemberInfo member, bool inherit)
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

        public static bool HasProperty(this Type type, string propertyName)
        {
            if (type == null)
                return false;

            var properties = GetProperties(type);
            return properties.Count(p => p.Name == propertyName) > 0;
        }
    }
}
