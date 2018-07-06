using System;
using System.Collections.Generic;

namespace Known.Mapping
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class EnumColumnAttribute : ColumnAttribute
    {
        public EnumColumnAttribute() { }

        public EnumColumnAttribute(string columnName, string description)
            : base(columnName, description)
        {
        }

        public override void Validate(object value, List<string> errors)
        {
            base.Validate(value, errors);
        }
    }
}
