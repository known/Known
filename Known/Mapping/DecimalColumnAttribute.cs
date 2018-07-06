using System;
using System.Collections.Generic;

namespace Known.Mapping
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DecimalColumnAttribute : ColumnAttribute
    {
        public DecimalColumnAttribute() { }

        public DecimalColumnAttribute(string columnName, string description, bool nullable, int integerLength, int decimalLength)
            : base(columnName, description, nullable)
        {
            IntegerLength = integerLength;
            DecimalLength = decimalLength;
        }

        public int? IntegerLength { get; set; }
        public int? DecimalLength { get; set; }

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
