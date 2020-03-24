using System;
using System.Collections.Generic;

namespace Known.Mapping
{
    /// <summary>
    /// 数值型栏位特性类。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DecimalColumnAttribute : ColumnAttribute
    {
        /// <summary>
        /// 初始化一个栏位特性类的实例。
        /// </summary>
        /// <param name="columnName">栏位名称。</param>
        /// <param name="description">栏位描述。</param>
        /// <param name="integerLength">整数位长度。</param>
        /// <param name="decimalLength">小数位长度。</param>
        /// <param name="required">是否必填，默认 False。</param>
        public DecimalColumnAttribute(string columnName, string description, int integerLength, int decimalLength, bool required = false)
            : base(columnName, description, required)
        {
            IntegerLength = integerLength;
            DecimalLength = decimalLength;
        }

        /// <summary>
        /// 取得整数位长度。
        /// </summary>
        public int IntegerLength { get; }

        /// <summary>
        /// 取得小数位长度。
        /// </summary>
        public int DecimalLength { get; }

        /// <summary>
        /// 验证栏位。
        /// </summary>
        /// <param name="value">栏位值。</param>
        /// <param name="errors">验证的错误信息。</param>
        public override void Validate(object value, List<string> errors)
        {
            base.Validate(value, errors);

            if (!Utils.IsNullOrEmpty(value))
            {
                if (!decimal.TryParse(value.ToString(), out decimal result))
                {
                    errors.Add($"{Description}必须是数值型！");
                }
                else
                {
                    var valueString = result.ToString();
                    valueString = valueString.Contains(".")
                                ? valueString.TrimEnd('0').TrimEnd('.')
                                : valueString;
                    var values = valueString.Split('.');
                    if (values.Length > 0 && values[0].Length > IntegerLength)
                    {
                        errors.Add($"{Description}最多为{IntegerLength}位整数！");
                    }
                    if (values.Length > 1 && values[1].Length > DecimalLength)
                    {
                        errors.Add($"{Description}最多为{DecimalLength}位小数！");
                    }
                }
            }
        }
    }
}
