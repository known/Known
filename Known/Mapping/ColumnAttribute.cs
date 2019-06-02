using System;
using System.Collections.Generic;

namespace Known.Mapping
{
    /// <summary>
    /// 栏位特性基类。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        /// <summary>
        /// 初始化一个栏位特性类的实例。
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="description"></param>
        /// <param name="required"></param>
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
