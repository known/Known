using System;
using System.Collections.Generic;

namespace Known.Mapping
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute() { }

        public ColumnAttribute(string columnName, string description)
        {
            ColumnName = columnName;
            Description = description;
        }

        public ColumnAttribute(string columnName, string description, bool nullable)
        {
            ColumnName = columnName;
            Description = description;
            Nullable = nullable;
        }

        public string ColumnName { get; set; }
        public string Description { get; set; }
        public bool Nullable { get; set; }

        public virtual void Validate(object value, List<string> errors)
        {
            if (!Nullable && Utils.IsNullOrEmpty(value))
            {
                errors.Add($"{Description}不能为空！");
            }
        }
    }
}
