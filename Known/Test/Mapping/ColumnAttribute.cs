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
        /// <param name="columnName">栏位名称。</param>
        /// <param name="description">栏位描述。</param>
        /// <param name="required">是否必填，默认 False。</param>
        public ColumnAttribute(string columnName, string description, bool required = false)
        {
            ColumnName = columnName;
            Description = description;
            Required = required;
        }

        /// <summary>
        /// 取得栏位名称。
        /// </summary>
        public string ColumnName { get; }

        /// <summary>
        /// 取得栏位描述。
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// 取得是否必填。
        /// </summary>
        public bool Required { get; }

        /// <summary>
        /// 验证栏位。
        /// </summary>
        /// <param name="value">栏位值。</param>
        /// <param name="errors">验证的错误信息。</param>
        public virtual void Validate(object value, List<string> errors)
        {
            if (Required && Utils.IsNullOrEmpty(value))
            {
                errors.Add($"{Description}不能为空！");
            }
        }
    }
}
