using System;
using System.ComponentModel;
using System.Reflection;

namespace Known.Extensions
{
    /// <summary>
    /// 枚举扩展类。
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 获取枚举类型值的DescriptionAttribute特性的描述内容。
        /// </summary>
        /// <param name="value">枚举类型值。</param>
        /// <returns>描述内容。</returns>
        public static string GetDescription(this Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            var field = type.GetField(name);
            var attr = GetAttribute<DescriptionAttribute>(field, false);
            return attr != null ? attr.Description : name;
        }

        private static T GetAttribute<T>(MemberInfo member, bool inherit = true)
        {
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
