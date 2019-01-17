using System.Linq;
using System.Reflection;
using Known.Extensions;

namespace Known.Mapping
{
    public class ColumnInfo
    {
        private ColumnAttribute attribute;

        public ColumnInfo(PropertyInfo property, string[] keys = null)
        {
            Property = property;
            IsKey = keys != null && keys.Contains(ColumnName);
        }

        public PropertyInfo Property { get; }
        public bool IsKey { get; }

        public ColumnAttribute Attribute
        {
            get
            {
                if (attribute != null)
                    return attribute;

                return Property.GetAttribute<ColumnAttribute>();
            }
            set { attribute = value; }
        }

        public string ColumnName
        {
            get { return Attribute != null ? Attribute.ColumnName : GetColumnName(Property); }
        }

        public string Title
        {
            get { return Attribute != null ? Attribute.Description : string.Empty; }
        }

        public string Description
        {
            get { return Attribute != null ? Attribute.Description : string.Empty; }
        }

        public bool Required
        {
            get { return Attribute != null ? Attribute.Required : false; }
        }

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
