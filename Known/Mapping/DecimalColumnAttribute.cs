using System;
using System.Collections.Generic;

namespace Known.Mapping
{
    /// <summary>
    /// 数值型表栏位特性，用于实体和表栏位的映射。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DecimalColumnAttribute : ColumnAttribute
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        public DecimalColumnAttribute() { }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="columnName">表栏位名。</param>
        /// <param name="description">表栏位描述。</param>
        /// <param name="nullable">是否可空。</param>
        /// <param name="integerLength">整数位数。</param>
        /// <param name="decimalLength">小数位数。</param>
        public DecimalColumnAttribute(string columnName, string description, bool nullable, int integerLength, int decimalLength)
            : base(columnName, description, nullable)
        {
            IntegerLength = integerLength;
            DecimalLength = decimalLength;
        }

        /// <summary>
        /// 取得或设置整数位数。
        /// </summary>
        public int? IntegerLength { get; set; }

        /// <summary>
        /// 取得或设置小数位数。
        /// </summary>
        public int? DecimalLength { get; set; }

        /// <summary>
        /// 验证实体属性值。
        /// </summary>
        /// <param name="value">属性值。</param>
        /// <param name="errors">错误集合。</param>
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
                    valueString = valueString.Contains(".") ? valueString.TrimEnd('0').TrimEnd('.') : valueString;
                    var values = valueString.Split('.');
                    if (IntegerLength.HasValue && values.Length > 0 && values[0].Length > IntegerLength.Value)
                    {
                        errors.Add($"{Description}最多为{IntegerLength.Value}位整数！");
                    }
                    if (DecimalLength.HasValue && values.Length > 1 && values[1].Length > DecimalLength.Value)
                    {
                        errors.Add($"{Description}最多为{DecimalLength.Value}位小数！");
                    }
                }
            }
        }
    }
}
