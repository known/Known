using System;
using System.Collections.Generic;
using System.Data;
using Known.Extensions;
using Known.Mapping;

namespace Known.Data
{
    /// <summary>
    /// 自动映射者类。
    /// </summary>
    public sealed class AutoMapper
    {
        /// <summary>
        /// 将 TSource 类型对象转成 T 类型的对象，只转两个类型相同属性名的栏位。
        /// </summary>
        /// <typeparam name="T">目标类型。</typeparam>
        /// <typeparam name="TSource">源类型。</typeparam>
        /// <param name="source">源类型对象。</param>
        /// <returns>目标类型对象。</returns>
        public static T MapTo<T, TSource>(TSource source)
        {
            if (source == null)
                return default;

            var json = source.ToJson();
            return json.FromJson<T>();
        }

        /// <summary>
        /// 将 TSource 类型集合对象转成 T 类型集合的对象，只转两个类型相同属性名的栏位。
        /// </summary>
        /// <typeparam name="T">目标类型。</typeparam>
        /// <typeparam name="TSource">源类型。</typeparam>
        /// <param name="sources">源类型对象集合。</param>
        /// <returns>目标类型对象集合。</returns>
        public static List<T> MapToList<T, TSource>(List<TSource> sources)
        {
            if (sources == null || sources.Count == 0)
                return null;

            var json = sources.ToJson();
            return json.FromJson<List<T>>();
        }

        internal static T GetBaseEntity<T>(DataRow row) where T : EntityBase
        {
            var entity = GetEntity<T>(row);
            entity.IsNew = false;
            return entity;
        }

        internal static List<T> GetBaseEntities<T>(DataTable data) where T : EntityBase
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

        internal static T GetEntity<T>(DataRow row)
        {
            if (row == null)
                return default;

            var entity = Activator.CreateInstance<T>();
            var columns = EntityHelper.GetColumnInfos<T>();
            foreach (var column in columns)
            {
                if (row.Table.Columns.Contains(column.ColumnName))
                {
                    var value = GetPropertyValue(column.Property.PropertyType, row[column.ColumnName]);
                    column.Property.SetValue(entity, value, null);
                }
            }

            return entity;
        }

        internal static List<T> GetEntities<T>(DataTable data)
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

            return Utils.ConvertTo(type, value);
        }
    }
}
