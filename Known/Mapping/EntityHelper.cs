using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Known.Extensions;

namespace Known.Mapping
{
    public sealed class EntityHelper
    {
        private static readonly ConcurrentDictionary<Type, EntityMapper> EntityMappers = new ConcurrentDictionary<Type, EntityMapper>();
        private static readonly ConcurrentDictionary<Type, List<PropertyInfo>> ColumnProperties = new ConcurrentDictionary<Type, List<PropertyInfo>>();
        private static readonly ConcurrentDictionary<Type, TableAttribute> TypeTables = new ConcurrentDictionary<Type, TableAttribute>();
        private static readonly ConcurrentDictionary<Type, List<ColumnInfo>> TypeColumns = new ConcurrentDictionary<Type, List<ColumnInfo>>();

        public static void InitMapper(Assembly assembly)
        {
            if (assembly == null)
                return;

            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsSubclassOf(typeof(EntityMapper))
                    && !type.IsAbstract
                    && !EntityMappers.ContainsKey(type))
                {
                    var mapper = Activator.CreateInstance(type) as EntityMapper;
                    EntityMappers[mapper.Type] = mapper;
                }
            }
        }

        public static string GetTableName<T>()
        {
            return GetTableName(typeof(T));
        }

        public static string GetTableName(Type type)
        {
            var attr = GetTableAttribute(type);
            return attr != null ? attr.TableName : type.Name;
        }

        public static TableAttribute GetTableAttribute<T>()
        {
            return GetTableAttribute(typeof(T));
        }

        public static TableAttribute GetTableAttribute(Type type)
        {
            if (type == null)
                return null;

            if (EntityMappers.TryGetValue(type, out EntityMapper mapper))
                return mapper.Table;

            if (TypeTables.TryGetValue(type, out TableAttribute attr))
                return attr;

            var attrs = type.GetCustomAttributes<TableAttribute>().ToList();
            if (attrs != null && attrs.Count > 0)
            {
                attr = attrs[0];
            }
            else
            {
                var name = type.Name.ToLower() + "s";
                attr = new TableAttribute(name, "");
            }

            TypeTables[type] = attr;
            return attr;
        }

        public static List<ColumnInfo> GetColumnInfos<T>()
        {
            return GetColumnInfos(typeof(T));
        }

        public static List<ColumnInfo> GetColumnInfos(Type type)
        {
            if (type == null)
                return null;

            if (EntityMappers.TryGetValue(type, out EntityMapper mapper))
                return mapper.Columns;

            if (TypeColumns.TryGetValue(type, out List<ColumnInfo> columns))
                return columns;

            var attrTable = GetTableAttribute(type);
            columns = type.GetColumnProperties()
                          .Select(p => new ColumnInfo(p, attrTable.PrimaryKeys))
                          .ToList();
            TypeColumns[type] = columns;
            return columns;
        }

        public static List<PropertyInfo> GetColumnProperties<T>()
        {
            return GetColumnProperties(typeof(T));
        }

        public static List<PropertyInfo> GetColumnProperties(Type type)
        {
            if (type == null)
                return null;

            if (EntityMappers.TryGetValue(type, out EntityMapper mapper))
                return mapper.Columns.Select(c => c.Property).ToList();

            if (ColumnProperties.TryGetValue(type, out List<PropertyInfo> pis))
                return pis.ToList();

            var properties = type.GetProperties()
                                 .Where(p => p.CanRead && p.CanWrite && !(p.SetMethod.IsVirtual && !p.SetMethod.IsFinal))
                                 .ToList();
            ColumnProperties[type] = properties;
            return properties.ToList();
        }

        public static Validator Validate<T>(T entity)
        {
            var infos = new List<ValidInfo>();
            var columns = GetColumnInfos<T>();
            foreach (var column in columns)
            {
                var errors = new List<string>();
                var value = column.Property.GetValue(entity, null);
                var attr = column.Attribute;
                if (attr != null)
                    attr.Validate(value, errors);

                if (errors.Count > 0)
                    infos.Add(new ValidInfo(ValidLevel.Error, column.Property.Name, errors));
            }
            return new Validator(infos);
        }

        public static void FillModel<T>(T entity, dynamic model)
        {
            var properties = GetColumnProperties<T>();
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
