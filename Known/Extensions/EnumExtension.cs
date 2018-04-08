using System;
using System.ComponentModel;

namespace Known.Extensions
{
    /// <summary>
    /// 枚举扩展类。
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 获取枚举的名称。
        /// </summary>
        /// <param name="value">枚举类型值。</param>
        /// <returns>枚举的名称。</returns>
        public static string GetName(this Enum value)
        {
            if (value == null)
                return string.Empty;

            var type = value.GetType();
            return Enum.GetName(type, value);
        }

        /// <summary>
        /// 获取枚举类型值的DescriptionAttribute特性的描述内容。
        /// </summary>
        /// <param name="value">枚举类型值。</param>
        /// <returns>描述内容。</returns>
        public static string GetDescription(this Enum value)
        {
            if (value == null)
                return string.Empty;

            var type = value.GetType();
            var name = Enum.GetName(type, value);
            var field = type.GetField(name);
            var attr = field.GetAttribute<DescriptionAttribute>(false);
            return attr != null ? attr.Description : name;
        }
    }
}
