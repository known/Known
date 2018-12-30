using System;
using System.Collections.Generic;

namespace Known.Mapping
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute(string columnName, string description, bool required = false)
        {
            ColumnName = columnName;
            Description = description;
            Required = required;
        }

        public string ColumnName { get; }
        public string Description { get; }
        public bool Required { get; }

        public virtual void Validate(object value, List<string> errors)
        {
            if (Required && Utils.IsNullOrEmpty(value))
            {
                errors.Add($"{Description}不能为空！");
            }
        }
    }
}
