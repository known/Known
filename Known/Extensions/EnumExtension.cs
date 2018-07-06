using System;
using System.ComponentModel;

namespace Known.Extensions
{
    public static class EnumExtension
    {
        public static string GetName(this Enum value)
        {
            if (value == null)
                return string.Empty;

            var type = value.GetType();
            return Enum.GetName(type, value);
        }

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
