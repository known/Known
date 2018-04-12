using System;
using System.Collections.Generic;
using Known.Extensions;
using Known.Validation;

namespace Known.Mapping
{
    /// <summary>
    /// 实体基类。
    /// </summary>
    public class EntityBase
    {
        /// <summary>
        /// 构造函数，创建一个实体类实例。
        /// </summary>
        public EntityBase()
        {
            IsNew = true;
        }

        /// <summary>
        /// 取得或设置ID。
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 取得或设置创建人。
        /// </summary>
        public string CreateBy { get; set; }

        /// <summary>
        /// 取得或设置创建时间。
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 取得或设置修改人。
        /// </summary>
        public string ModifyBy { get; set; }

        /// <summary>
        /// 取得或设置修改时间。
        /// </summary>
        public DateTime? ModifyTime { get; set; }

        /// <summary>
        /// 取得或设置是否为新建实体。
        /// </summary>
        internal bool IsNew { get; set; }

        /// <summary>
        /// 验证实体数据。
        /// </summary>
        /// <returns>验证器。</returns>
        public Validator Validate()
        {
            var errors = new List<string>();
            var properties = GetType().GetColumnProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(this, null);
                var attr = property.GetAttribute<ColumnAttribute>();
                if (attr != null)
                    attr.Validate(value, errors);
            }
            return new Validator(errors);
        }
    }
}
