using System.Linq;
using System.Reflection;
using Known.Extensions;

namespace Known.Mapping
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
        public ColumnAttribute Attribute { get; set; }
        public string ColumnName { get; }
        public string Title { get; }
        public string Description { get; }
        public bool IsKey { get; }
        public bool Required { get; }
        public bool Exportable { get; }

        private static string GetColumnName(PropertyInfo property)
        {
            var attr = property.GetAttribute<ColumnAttribute>();
            if (attr != null)
                return attr.ColumnName;

            if (property.PropertyType.IsSubclassOf(typeof(EntityBase)))
                return property.Name.ToLower() + "_id";

            return property.Name.ToLower();
        }
    }
}
