using System;
using System.Collections.Generic;
using System.Data;
using Known.Extensions;
using Known.Mapping;

namespace Known.Data
{
    /// <summary>
    /// 自动映射器。
    /// </summary>
    public class AutoMapper
    {
        /// <summary>
        /// 根据数据行自动映射实体对象。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="row">数据行。</param>
        /// <returns>实体对象。</returns>
        public static T GetBaseEntity<T>(DataRow row) where T : EntityBase
        {
            var entity = GetEntity<T>(row);
            entity.IsNew = false;
            return entity;
        }

        /// <summary>
        /// 根据数据表自动映射实体对象列表。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="data">数据表。</param>
        /// <returns>实体对象列表。</returns>
        public static List<T> GetBaseEntities<T>(DataTable data) where T : EntityBase
        {
            if (data == null || data.Rows.Count == 0)
                return null;

            var lists = new List<T>();
            foreach (DataRow row in data.Rows)
            {
                lists.Add(GetBaseEntity<T>(row));
            }
            return lists;
        }

        /// <summary>
        /// 根据数据行自动映射对象。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="row">数据行。</param>
        /// <returns>对象。</returns>
        public static T GetEntity<T>(DataRow row)
        {
            if (row == null)
                return default(T);

            var entity = Activator.CreateInstance<T>();
            var properties = typeof(T).GetColumnProperties();
            foreach (var property in properties)
            {
                var columnName = ColumnInfo.GetColumnName(property);
                if (row.Table.Columns.Contains(columnName))
                {
                    var value = GetPropertyValue(property.PropertyType, row[columnName]);
                    property.SetValue(entity, value, null);
                }
            }

            return entity;
        }

        /// <summary>
        /// 根据数据表自动映射对象列表。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="data">数据表。</param>
        /// <returns>对象列表。</returns>
        public static List<T> GetEntities<T>(DataTable data)
        {
            if (data == null || data.Rows.Count == 0)
                return null;

            var lists = new List<T>();
            foreach (DataRow row in data.Rows)
            {
                lists.Add(GetEntity<T>(row));
            }
            return lists;
        }

        private static object GetPropertyValue(Type type, object value)
        {
            if (type.IsSubclassOf(typeof(EntityBase)))
            {
                var entity = Activator.CreateInstance(type) as EntityBase;
                entity.Id = value.ToString();
                entity.IsNew = false;
                return entity;
            }

            return value;
        }
    }
}
