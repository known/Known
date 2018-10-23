using System;
using System.Collections.Generic;

namespace Known.Mapping
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DecimalColumnAttribute : ColumnAttribute
    {
        public DecimalColumnAttribute(string columnName, string description, int integerLength, int decimalLength, bool required = false)
            : base(columnName, description, required)
        {
            IntegerLength = integerLength;
            DecimalLength = decimalLength;
        }

        public int IntegerLength { get; }
        public int DecimalLength { get; }

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
