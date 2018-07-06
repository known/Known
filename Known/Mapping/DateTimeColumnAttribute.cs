using System;
using System.Collections.Generic;
using Known.Extensions;

namespace Known.Mapping
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DateTimeColumnAttribute : ColumnAttribute
    {
        public DateTimeColumnAttribute() { }

        public DateTimeColumnAttribute(string columnName, string description, bool nullable)
            : base(columnName, description, nullable)
        {
        }

        public DateTimeColumnAttribute(string columnName, string description, bool nullable, string format)
            : base(columnName, description, nullable)
        {
            Format = format;
        }

        public DateTimeColumnAttribute(string columnName, string description, bool nullable, DateTime? minValue, DateTime? maxValue)
            : base(columnName, description, nullable)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public DateTime? MinValue { get; set; }
        public DateTime? MaxValue { get; set; }
        public string Format { get; set; }

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
