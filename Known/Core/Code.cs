using System;
using System.Collections.Generic;
using Known.Extensions;

namespace Known.Core
{
    /// <summary>
    /// 系统代码信息类。
    /// </summary>
    public class Code
    {
        /// <summary>
        /// 取得或设置 id。
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 取得或设置代码文本。
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// 取得或设置代码扩展对象。
        /// </summary>
        public object ext { get; set; }

        public static Dictionary<string, object> GetCodes(PlatformService service)
        {
            var codes = service.GetCodes();
            if (codes == null)
                codes = new Dictionary<string, object>();
            codes.Add("ViewType", GetEnumCodes<ViewType>());
            return codes;
        }

        /// <summary>
        /// 获取指定枚举类型的代码列表。
        /// </summary>
        /// <typeparam name="T">枚举类型。</typeparam>
        /// <returns>代码列表。</returns>
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

        /// <summary>
        /// 获取指定字符串数组的代码列表。
        /// </summary>
        /// <param name="values">字符串数组。</param>
        /// <returns>代码列表。</returns>
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