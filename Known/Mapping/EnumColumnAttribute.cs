using System;
using System.Collections.Generic;

namespace Known.Mapping
{
    /// <summary>
    /// 枚举型表栏位特性，用于实体和表栏位的映射。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class EnumColumnAttribute : ColumnAttribute
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        public EnumColumnAttribute() { }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="columnName">表栏位名。</param>
        /// <param name="description">表栏位描述。</param>
        public EnumColumnAttribute(string columnName, string description)
            : base(columnName, description)
        {
        }

        /// <summary>
        /// 验证实体属性值。
        /// </summary>
        /// <param name="value">属性值。</param>
        /// <param name="errors">错误集合。</param>
        public override void Validate(object value, List<string> errors)
        {
            base.Validate(value, errors);
        }
    }
}
