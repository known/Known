using System;
using System.Collections.Generic;

namespace Known.Mapping
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class IntegerColumnAttribute : ColumnAttribute
    {
        public IntegerColumnAttribute() { }

        public IntegerColumnAttribute(string columnName, string description, bool nullable)
            : base(columnName, description, nullable)
        {
        }

        public override void Validate(object value, List<string> errors)
        {
            base.Validate(value, errors);

            if (!Utils.IsNullOrEmpty(value))
            {
                if (!int.TryParse(value.ToString(), out int result))
                {
                    errors.Add($"{Description}必须是整数！");
                }
            }
        }
    }
}
