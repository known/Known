using System;
using System.Collections.Generic;
using Known.Extensions;

namespace Known.Mapping
{
    /// <summary>
    /// 日期时间型栏位特性类。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DateTimeColumnAttribute : ColumnAttribute
    {
        /// <summary>
        /// 初始化一个日期时间型栏位特性类的实例。
        /// </summary>
        /// <param name="columnName">栏位名称。</param>
        /// <param name="description">栏位描述。</param>
        /// <param name="required">是否必填，默认 False。</param>
        public DateTimeColumnAttribute(string columnName, string description, bool required = false)
            : base(columnName, description, required)
        {
        }

        /// <summary>
        /// 初始化一个日期时间型栏位特性类的实例。
        /// </summary>
        /// <param name="columnName">栏位名称。</param>
        /// <param name="description">栏位描述。</param>
        /// <param name="format">日期时间格式。</param>
        /// <param name="required">是否必填，默认 False。</param>
        public DateTimeColumnAttribute(string columnName, string description, string format, bool required = false)
            : base(columnName, description, required)
        {
            Format = format;
        }

        /// <summary>
        /// 初始化一个日期时间型栏位特性类的实例。
        /// </summary>
        /// <param name="columnName">栏位名称。</param>
        /// <param name="description">栏位描述。</param>
        /// <param name="minValue">最小值。</param>
        /// <param name="maxValue">最大值。</param>
        /// <param name="required">是否必填，默认 False。</param>
        public DateTimeColumnAttribute(string columnName, string description, DateTime? minValue, DateTime? maxValue, bool required = false)
            : base(columnName, description, required)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        /// <summary>
        /// 取得最小值。
        /// </summary>
        public DateTime? MinValue { get; }

        /// <summary>
        /// 取得最大值。
        /// </summary>
        public DateTime? MaxValue { get; }

        /// <summary>
        /// 取得日期时间格式。
        /// </summary>
        public string Format { get; }

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
