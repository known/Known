using System;
using System.Collections.Generic;

namespace Known.Mapping
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class EnumColumnAttribute : ColumnAttribute
    {
        public EnumColumnAttribute(string columnName, string description)
            : base(columnName, description, true)
        {
        }

        public override void Validate(object value, List<string> errors)
        {
            base.Validate(value, errors);
        }
    }
}
