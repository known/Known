using System.Linq;
using System.Reflection;
using Known.Extensions;
using Known.Mapping;

namespace Known.Data
{
    public class ColumnInfo
    {
        public ColumnInfo(PropertyInfo property, string[] keys = null)
        {
            Property = property;
            ColumnName = GetColumnName(property);
            IsKey = keys != null && keys.Contains(ColumnName);
        }

        public PropertyInfo Property { get; }
        public string ColumnName { get; }
        public bool IsKey { get; }

        private static string GetColumnName(PropertyInfo property)
        {
            var attr = property.GetAttribute<ColumnAttribute>();
            if (attr != null)
                return attr.ColumnName;

            if (property.PropertyType.IsSubclassOf(typeof(BaseEntity)))
                return property.Name.ToLower() + "_id";

            return property.Name.ToLower();
        }
    }
}
