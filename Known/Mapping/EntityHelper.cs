using System.Collections.Generic;
using System.Linq;
using Known.Extensions;

namespace Known.Mapping
{
    public sealed class EntityHelper
    {
        public static Validator Validate<T>(T entity)
        {
            var infos = new List<ValidInfo>();
            var properties = typeof(T).GetColumnProperties();
            foreach (var property in properties)
            {
                var errors = new List<string>();
                var value = property.GetValue(entity, null);
                var attr = property.GetAttribute<ColumnAttribute>();
                if (attr != null)
                    attr.Validate(value, errors);

                if (errors.Count > 0)
                    infos.Add(new ValidInfo(ValidLevel.Error, property.Name, errors));
            }
            return new Validator(infos);
        }

        public static void FillModel<T>(T entity, dynamic model)
        {
            var properties = typeof(T).GetColumnProperties();
            var pis = model.Properties();
            foreach (var pi in pis)
            {
                var name = (string)pi.Name;
                if (name == "Id")
                    continue;

                var value = (object)pi.Value.Value;
                var property = properties.FirstOrDefault(p => p.Name == name);
                if (property != null)
                {
                    value = Utils.ConvertTo(property.PropertyType, value);
                    property.SetValue(entity, value);
                }
            }
        }
    }
}
