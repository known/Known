using System.Linq;
using System.Reflection;
using Known.Extensions;

namespace Known.Mapping
{
    /// <summary>
    /// 数据栏位信息类。
    /// </summary>
    public class ColumnInfo
    {
        private ColumnAttribute attribute;

        /// <summary>
        /// 初始化一个数据栏位信息类的实例。
        /// </summary>
        /// <param name="property">属性对象。</param>
        /// <param name="keys">主键数组。</param>
        public ColumnInfo(PropertyInfo property, string[] keys = null)
        {
            Property = property;
            IsKey = keys != null && keys.Contains(ColumnName);
        }

        /// <summary>
        /// 取得栏位属性对象。
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// 取得栏位是否是主键。
        /// </summary>
        public bool IsKey { get; }

        /// <summary>
        /// 取得栏位特性对象。
        /// </summary>
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

        /// <summary>
        /// 取得数据栏位字段名。
        /// </summary>
        public string ColumnName
        {
            get { return Attribute != null ? Attribute.ColumnName : GetColumnName(Property); }
        }

        /// <summary>
        /// 取得数据栏位标题。
        /// </summary>
        public string Title
        {
            get { return Attribute != null ? Attribute.Description : string.Empty; }
        }

        /// <summary>
        /// 取得数据栏位描述。
        /// </summary>
        public string Description
        {
            get { return Attribute != null ? Attribute.Description : string.Empty; }
        }

        /// <summary>
        /// 取得数据栏位是否为必填。
        /// </summary>
        public bool Required
        {
            get { return Attribute != null ? Attribute.Required : false; }
        }

        /// <summary>
        /// 取得数据栏位是否可导出。
        /// </summary>
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
