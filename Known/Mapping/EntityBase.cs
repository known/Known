using System;
using System.Collections.Generic;
using Known.Extensions;
using Known.Validation;

namespace Known.Mapping
{
    public class EntityBase
    {
        public EntityBase()
        {
            IsNew = true;
        }

        public string Id { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateTime { get; set; }
        public string ModifyBy { get; set; }
        public DateTime? ModifyTime { get; set; }
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
