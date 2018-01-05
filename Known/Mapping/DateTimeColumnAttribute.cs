using Known.Extensions;
using System;
using System.Collections.Generic;

namespace Known.Mapping
{
    /// <summary>
    /// 日期时间型表栏位特性，用于实体和表栏位的映射。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DateTimeColumnAttribute : ColumnAttribute
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        public DateTimeColumnAttribute() { }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="columnName">表栏位名。</param>
        /// <param name="description">表栏位描述。</param>
        /// <param name="nullable">是否可空。</param>
        public DateTimeColumnAttribute(string columnName, string description, bool nullable)
            : base(columnName, description, nullable)
        {
        }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="columnName">表栏位名。</param>
        /// <param name="description">表栏位描述。</param>
        /// <param name="nullable">是否可空。</param>
        /// <param name="format">日期格式。</param>
        public DateTimeColumnAttribute(string columnName, string description, bool nullable, string format)
            : base(columnName, description, nullable)
        {
            Format = format;
        }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="columnName">表栏位名。</param>
        /// <param name="description">表栏位描述。</param>
        /// <param name="nullable">是否可空。</param>
        /// <param name="minValue">最小值。</param>
        /// <param name="maxValue">最大值。</param>
        public DateTimeColumnAttribute(string columnName, string description, bool nullable, DateTime? minValue, DateTime? maxValue)
            : base(columnName, description, nullable)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        /// <summary>
        /// 取得或设置最小值。
        /// </summary>
        public DateTime? MinValue { get; set; }

        /// <summary>
        /// 取得或设置最大值。
        /// </summary>
        public DateTime? MaxValue { get; set; }

        /// <summary>
        /// 取得或设置日期格式。
        /// </summary>
        public string Format { get; set; }

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
