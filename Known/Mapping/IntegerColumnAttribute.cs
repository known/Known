using System;
using System.Collections.Generic;

namespace Known.Mapping
{
    /// <summary>
    /// 整型栏位特性类。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class IntegerColumnAttribute : ColumnAttribute
    {
        /// <summary>
        /// 初始化一个整型栏位特性类的实例。
        /// </summary>
        /// <param name="columnName">栏位名称。</param>
        /// <param name="description">栏位描述。</param>
        /// <param name="required">是否必填，默认 False。</param>
        public IntegerColumnAttribute(string columnName, string description, bool required = false)
            : base(columnName, description, required)
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
