using System;
using System.Collections.Generic;

namespace Known.Mapping
{
    /// <summary>
    /// 数据表栏位特性，用于实体和表栏位的映射。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        public ColumnAttribute() { }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="columnName">表栏位名。</param>
        /// <param name="description">表栏位描述。</param>
        public ColumnAttribute(string columnName, string description)
        {
            ColumnName = columnName;
            Description = description;
        }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="columnName">表栏位名。</param>
        /// <param name="description">表栏位描述。</param>
        /// <param name="nullable">是否可空。</param>
        public ColumnAttribute(string columnName, string description, bool nullable)
        {
            ColumnName = columnName;
            Description = description;
            Nullable = nullable;
        }

        /// <summary>
        /// 取得或设置表栏位名。
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 取得或设置表栏位描述。
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 取得或设置是否可空。
        /// </summary>
        public bool Nullable { get; set; }

        /// <summary>
        /// 验证实体属性值。
        /// </summary>
        /// <param name="value">属性值。</param>
        /// <param name="errors">错误集合。</param>
        public virtual void Validate(object value, List<string> errors)
        {
            if (!Nullable && Utils.IsNullOrEmpty(value))
            {
                errors.Add($"{Description}不能为空！");
            }
        }
    }
}
