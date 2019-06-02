using System;
using System.Collections.Generic;
using Known.Extensions;

namespace Known.Mapping
{
    /// <summary>
    /// 字符型栏位特性类。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class StringColumnAttribute : ColumnAttribute
    {
        /// <summary>
        /// 初始化一个整型栏位特性类的实例。
        /// </summary>
        /// <param name="columnName">栏位名称。</param>
        /// <param name="description">栏位描述。</param>
        /// <param name="required">是否必填，默认 False。</param>
        public StringColumnAttribute(string columnName, string description, bool required = false)
            : base(columnName, description, required)
        {
        }

        /// <summary>
        /// 初始化一个整型栏位特性类的实例。
        /// </summary>
        /// <param name="columnName">栏位名称。</param>
        /// <param name="description">栏位描述。</param>
        /// <param name="length">固定长度。</param>
        /// <param name="required">是否必填，默认 False。</param>
        public StringColumnAttribute(string columnName, string description, int length, bool required = false)
            : base(columnName, description, required)
        {
            Length = length;
        }

        /// <summary>
        /// 初始化一个整型栏位特性类的实例。
        /// </summary>
        /// <param name="columnName">栏位名称。</param>
        /// <param name="description">栏位描述。</param>
        /// <param name="minLength">最小长度。</param>
        /// <param name="maxLength">最大长度。</param>
        /// <param name="required">是否必填，默认 False。</param>
        public StringColumnAttribute(string columnName, string description, int minLength, int maxLength, bool required = false)
            : base(columnName, description, required)
        {
            MinLength = minLength;
            MaxLength = maxLength;
        }

        /// <summary>
        /// 取得固定长度。
        /// </summary>
        public int? Length { get; }

        /// <summary>
        /// 取得最小长度。
        /// </summary>
        public int? MinLength { get; }

        /// <summary>
        /// 取得最大长度。
        /// </summary>
        public int? MaxLength { get; }

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
                var valueString = value.ToString().Trim();
                var length = valueString.ByteLength();
                if (Length.HasValue && length != Length.Value)
                {
                    errors.Add($"{Description}必须为{Length.Value}位字符！");
                }
                if (MinLength.HasValue && length < MinLength.Value)
                {
                    errors.Add($"{Description}最少为{MinLength.Value}位字符！");
                }
                if (MaxLength.HasValue && length > MaxLength.Value)
                {
                    errors.Add($"{Description}最多为{MaxLength.Value}位字符！");
                }
            }
        }
    }
}
