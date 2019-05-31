using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Known.Extensions
{
    /// <summary>
    /// 枚举值扩展类。
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 获取枚举值的名称。
        /// </summary>
        /// <param name="value">枚举值。</param>
        /// <returns>枚举值的名称。</returns>
        public static string GetName(this Enum value)
        {
            if (value == null)
                return string.Empty;

            var type = value.GetType();
            return Enum.GetName(type, value);
        }

        /// <summary>
        /// 获取枚举值的描述。
        /// </summary>
        /// <param name="value">枚举值。</param>
        /// <returns>枚举值的描述。</returns>
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

        /// <summary>
        /// 获取枚举类型的字典。
        /// </summary>
        /// <param name="enumType">枚举类型。</param>
        /// <returns>枚举类型的字典。</returns>
        public static Dictionary<int, string> ToDictionary(this Type enumType)
        {
            var codes = new Dictionary<int, string>();
            var values = Enum.GetValues(enumType);
            foreach (Enum value in values)
            {
                var code = Convert.ToInt32(value);
                var name = value.GetDescription();
                codes.Add(code, name);
            }
            return codes;
        }
    }
}
