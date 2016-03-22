using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Known.Extensions
{
    public static class EnumExtension
    {
        public static List<CodeInfo> ToCodes(this Type enumType)
        {
            var category = enumType.Name;
            var codes = new List<CodeInfo>();
            var values = Enum.GetValues(enumType);
            foreach (Enum value in values)
            {
                var code = Convert.ToInt32(value).ToString();
                var name = value.GetDescription();
                codes.Add(new CodeInfo(category, code, name));
            }
            return codes;
        }

        public static string GetDescription(this Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            var field = type.GetField(name);
            var attr = field.GetAttribute<DescriptionAttribute>(false);
            return attr != null ? attr.Description : name;
        }

        public static string Name(this Enum value)
        {
            var type = value.GetType();
            return Enum.GetName(type, value);
        }

        public static T Value<T>(this Enum value)
        {
            var val = Convert.ToInt32(value);
            return val.To<T>();
        }
    }
}
