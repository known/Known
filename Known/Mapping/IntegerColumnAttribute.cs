using System;
using System.Collections.Generic;

namespace Known.Mapping
{
    /// <summary>
    /// 整数型表栏位特性，用于实体和表栏位的映射。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class IntegerColumnAttribute : ColumnAttribute
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        public IntegerColumnAttribute() { }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="columnName">表栏位名。</param>
        /// <param name="description">表栏位描述。</param>
        /// <param name="nullable">是否可空。</param>
        public IntegerColumnAttribute(string columnName, string description, bool nullable)
            : base(columnName, description, nullable)
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
