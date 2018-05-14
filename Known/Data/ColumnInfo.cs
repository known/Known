using System.Reflection;
using Known.Extensions;
using Known.Mapping;

namespace Known.Data
{
    /// <summary>
    /// 数据库栏位信息。
    /// </summary>
    public class ColumnInfo
    {
        /// <summary>
        /// 取得或设置是否是主键。
        /// </summary>
        public bool IsKey { get; set; }

        /// <summary>
        /// 取得或设置栏位名。
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 取得或设置栏位对应的实体属性。
        /// </summary>
        public PropertyInfo Property { get; set; }

        /// <summary>
        /// 获取实体属性映射的数据库栏位名。
        /// </summary>
        /// <param name="property">实体属性。</param>
        /// <returns>数据库栏位名。</returns>
        public static string GetColumnName(PropertyInfo property)
        {
            var attr = property.GetAttribute<ColumnAttribute>();
            if (attr != null)
                return attr.ColumnName;

            if (property.PropertyType.IsSubclassOf(typeof(EntityBase)))
                return property.Name + "Id";

            return property.Name;
        }
    }
}
