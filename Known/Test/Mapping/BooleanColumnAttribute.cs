using System;
using System.Collections.Generic;

namespace Known.Mapping
{
    /// <summary>
    /// 布尔型栏位特性类。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class BooleanColumnAttribute : ColumnAttribute
    {
        /// <summary>
        /// 初始化一个布尔型栏位特性类的实例。
        /// </summary>
        /// <param name="columnName">栏位名称。</param>
        /// <param name="description">栏位描述。</param>
        public BooleanColumnAttribute(string columnName, string description)
            : base(columnName, description, true)
        {
        }

        /// <summary>
        /// 验证栏位。
        /// </summary>
        /// <param name="value">栏位值。</param>
        /// <param name="errors">验证的错误信息。</param>
        public override void Validate(object value, List<string> errors)
        {
            base.Validate(value, errors);
        }
    }
}
