using System;
using System.Collections.Generic;
using Known.Extensions;

namespace Known.Mapping
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class StringColumnAttribute : ColumnAttribute
    {
        public StringColumnAttribute() { }

        public StringColumnAttribute(string columnName, string description, bool nullable, int length)
            : base(columnName, description, nullable)
        {
            Length = length;
        }

        public StringColumnAttribute(string columnName, string description, bool nullable, int minLength, int maxLength)
            : base(columnName, description, nullable)
        {
            MinLength = minLength;
            MaxLength = maxLength;
        }

        public int? Length { get; set; }
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }

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
