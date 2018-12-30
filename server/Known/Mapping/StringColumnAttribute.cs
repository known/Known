using System;
using System.Collections.Generic;
using Known.Extensions;

namespace Known.Mapping
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class StringColumnAttribute : ColumnAttribute
    {
        public StringColumnAttribute(string columnName, string description, bool required = false)
            : base(columnName, description, required)
        {
        }

        public StringColumnAttribute(string columnName, string description, int length, bool required = false)
            : base(columnName, description, required)
        {
            Length = length;
        }

        public StringColumnAttribute(string columnName, string description, int minLength, int maxLength, bool required = false)
            : base(columnName, description, required)
        {
            MinLength = minLength;
            MaxLength = maxLength;
        }

        public int? Length { get; }
        public int? MinLength { get; }
        public int? MaxLength { get; }

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
