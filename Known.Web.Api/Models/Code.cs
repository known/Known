using System;
using System.Collections.Generic;
using Known.Extensions;

namespace Known.Platform.WebApi.Models
{
    public class Code
    {
        public string id { get; set; }
        public string text { get; set; }

        public static List<Code> GetEnumCodes<T>()
        {
            var codes = new List<Code>();
            var enumType = typeof(T);
            var values = Enum.GetValues(enumType);

            foreach (Enum value in values)
            {
                var id = Convert.ToInt32(value).ToString();
                var text = value.GetDescription();
                if (string.IsNullOrWhiteSpace(text))
                {
                    text = Enum.GetName(enumType, value);
                }

                codes.Add(new Code { id = id, text = text });
            }

            return codes;
        }

        public static List<Code> GetStringCodes(params string[] values)
        {
            var codes = new List<Code>();

            foreach (var item in values)
            {
                codes.Add(new Code { id = item, text = item });
            }

            return codes;
        }
    }
}