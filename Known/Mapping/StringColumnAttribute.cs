using Known.Extensions;
using System;
using System.Collections.Generic;

namespace Known.Mapping
{
    /// <summary>
    /// 字符型表栏位特性，用于实体和表栏位的映射。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class StringColumnAttribute : ColumnAttribute
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        public StringColumnAttribute() { }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="columnName">表栏位名。</param>
        /// <param name="description">表栏位描述。</param>
        /// <param name="nullable">是否可空。</param>
        /// <param name="length">固定长度。</param>
        public StringColumnAttribute(string columnName, string description, bool nullable, int length)
            : base(columnName, description, nullable)
        {
            Length = length;
        }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="columnName">表栏位名。</param>
        /// <param name="description">表栏位描述。</param>
        /// <param name="nullable">是否可空。</param>
        /// <param name="minLength">最小长度。</param>
        /// <param name="maxLength">最大长度。</param>
        public StringColumnAttribute(string columnName, string description, bool nullable, int minLength, int maxLength)
            : base(columnName, description, nullable)
        {
            MinLength = minLength;
            MaxLength = maxLength;
        }

        /// <summary>
        /// 取得或设置固定长度。
        /// </summary>
        public int? Length { get; set; }

        /// <summary>
        /// 取得或设置最小长度。
        /// </summary>
        public int? MinLength { get; set; }

        /// <summary>
        /// 取得或设置最大长度。
        /// </summary>
        public int? MaxLength { get; set; }

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
