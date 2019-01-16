using System;
using System.Collections.Generic;
using System.Data;
using Known.Extensions;
using Known.Mapping;

namespace Known.Data
{
    public sealed class AutoMapper
    {
        public static T MapTo<T, TSource>(TSource source)
        {
            if (source == null)
                return default(T);

            var json = source.ToJson();
            return json.FromJson<T>();
        }

        public static List<T> MapToList<T, TSource>(List<TSource> sources)
        {
            if (sources == null || sources.Count == 0)
                return null;

            var json = sources.ToJson();
            return json.FromJson<List<T>>();
        }

        public static T GetBaseEntity<T>(DataRow row) where T : EntityBase
        {
            var entity = GetEntity<T>(row);
            entity.IsNew = false;
            return entity;
        }

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

        public static T GetEntity<T>(DataRow row)
        {
            if (row == null)
                return default(T);

            var entity = Activator.CreateInstance<T>();
            var properties = typeof(T).GetColumnProperties();
            foreach (var property in properties)
            {
                var column = new ColumnInfo(property);
                if (row.Table.Columns.Contains(column.ColumnName))
                {
                    var value = GetPropertyValue(property.PropertyType, row[column.ColumnName]);
                    property.SetValue(entity, value, null);
                }
            }

            return entity;
        }

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

            return Utils.ConvertTo(type, value);
        }
    }
}
