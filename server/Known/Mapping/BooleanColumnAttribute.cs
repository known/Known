using System;
using System.Collections.Generic;

namespace Known.Mapping
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class BooleanColumnAttribute : ColumnAttribute
    {
        public BooleanColumnAttribute(string columnName, string description)
            : base(columnName, description, true)
        {
        }

        public override void Validate(object value, List<string> errors)
        {
            base.Validate(value, errors);
        }
    }
}
