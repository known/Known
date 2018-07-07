using System.Reflection;
using Known.Extensions;
using Known.Mapping;

namespace Known.Data
{
    public class ColumnInfo
    {
        public bool IsKey { get; set; }
        public string ColumnName { get; set; }
        public PropertyInfo Property { get; set; }

        public static string GetColumnName(PropertyInfo property)
        {
            var attr = property.GetAttribute<ColumnAttribute>();
            if (attr != null)
                return attr.ColumnName;

            if (property.PropertyType.IsSubclassOf(typeof(BaseEntity)))
                return property.Name + "Id";

            return property.Name;
        }
    }
}
