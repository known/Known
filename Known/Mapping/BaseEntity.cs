using System;
using System.Collections.Generic;
using Known.Extensions;
using Known.Validation;

namespace Known.Mapping
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            IsNew = true;
        }

        [StringColumn("id", "主键", 1, 50, true)]
        public string Id { get; set; }

        [StringColumn("create_by", "创建人", 1, 50, true)]
        public string CreateBy { get; set; }

        [DateTimeColumn("create_time", "创建时间", true)]
        public DateTime CreateTime { get; set; }

        [StringColumn("modify_by", "修改人", 1, 50)]
        public string ModifyBy { get; set; }

        [DateTimeColumn("modify_time", "修改时间")]
        public DateTime? ModifyTime { get; set; }

        [StringColumn("extension", "扩展属性")]
        public string Extension { get; set; }

        internal bool IsNew { get; set; }

        public Validator Validate()
        {
            var infos = new List<ValidInfo>();
            var properties = GetType().GetColumnProperties();
            foreach (var property in properties)
            {
                var errors = new List<string>();
                var value = property.GetValue(this, null);
                var attr = property.GetAttribute<ColumnAttribute>();
                if (attr != null)
                    attr.Validate(value, errors);

                if (errors.Count > 0)
                    infos.Add(new ValidInfo(ValidLevel.Error, property.Name, errors));
            }
            return new Validator(infos);
        }
    }
}
