using System;
using System.Collections.Generic;
using Known.Extensions;

namespace Known.Mapping
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DateTimeColumnAttribute : ColumnAttribute
    {
        public DateTimeColumnAttribute(string columnName, string description, bool required = false)
            : base(columnName, description, required)
        {
        }

        public DateTimeColumnAttribute(string columnName, string description, string format, bool required = false)
            : base(columnName, description, required)
        {
            Format = format;
        }

        public DateTimeColumnAttribute(string columnName, string description, DateTime? minValue, DateTime? maxValue, bool required = false)
            : base(columnName, description, required)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public DateTime? MinValue { get; }
        public DateTime? MaxValue { get; }
        public string Format { get; }

        public override void Validate(object value, List<string> errors)
        {
            base.Validate(value, errors);

            if (!Utils.IsNullOrEmpty(value))
            {
                DateTime result;
                if (string.IsNullOrWhiteSpace(Format))
                {
                    if (!DateTime.TryParse(value.ToString(), out result))
                    {
                        errors.Add($"{Description}必须是日期时间型！");
                        return;
                    }
                }
                else
                {
                    var dt = value.ToString().ToDateTime(Format);
                    if (!dt.HasValue)
                    {
                        errors.Add($"{Description}必须是{Format}格式的日期时间型！");
                        return;
                    }
                    result = dt.Value;
                }

                if (MinValue.HasValue && result < MinValue.Value)
                {
                    errors.Add($"{Description}最小日期时间为{MinValue.Value}！");
                }
                if (MaxValue.HasValue && result > MaxValue.Value)
                {
                    errors.Add($"{Description}最大日期时间为{MaxValue.Value}！");
                }
            }
        }
    }
}
