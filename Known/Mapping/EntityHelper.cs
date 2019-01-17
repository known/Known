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
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, EntityMapper>
            EntityMappers = new ConcurrentDictionary<RuntimeTypeHandle, EntityMapper>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> 
            ColumnProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, TableAttribute> 
            TypeTables = new ConcurrentDictionary<RuntimeTypeHandle, TableAttribute>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<ColumnInfo>> 
            TypeColumns = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<ColumnInfo>>();

        public static void InitMapper(Assembly assembly)
        {
            if (assembly == null)
                return;

            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsSubclassOf(typeof(EntityMapper))
                    && !type.IsAbstract
                    && !EntityMappers.ContainsKey(type.TypeHandle))
                {
                    var mapper = Activator.CreateInstance(type);
                    EntityMappers[type.TypeHandle] = mapper as EntityMapper;
                }
            }
        }

        public static TableAttribute GetTableAttribute(Type type)
        {
            if (type == null)
                return null;

            if (EntityMappers.TryGetValue(type.TypeHandle, out EntityMapper mapper))
                return mapper.Table;

            if (TypeTables.TryGetValue(type.TypeHandle, out TableAttribute attr))
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

            TypeTables[type.TypeHandle] = attr;
            return attr;
        }

        public static IEnumerable<ColumnInfo> GetColumnInfos(Type type)
        {
            if (type == null)
                return null;

            if (EntityMappers.TryGetValue(type.TypeHandle, out EntityMapper mapper))
                return mapper.Columns;

            if (TypeColumns.TryGetValue(type.TypeHandle, out IEnumerable<ColumnInfo> columns))
                return columns;

            var attrTable = GetTableAttribute(type);
            columns = type.GetColumnProperties()
                          .Select(p => new ColumnInfo(p, attrTable.PrimaryKeys));
            TypeColumns[type.TypeHandle] = columns;
            return columns;
        }

        public static List<PropertyInfo> GetColumnProperties(Type type)
        {
            if (type == null)
                return null;

            if (EntityMappers.TryGetValue(type.TypeHandle, out EntityMapper mapper))
                return mapper.Columns.Select(c => c.Property).ToList();

            if (ColumnProperties.TryGetValue(type.TypeHandle, out IEnumerable<PropertyInfo> pis))
                return pis.ToList();

            var properties = type.GetProperties()
                                 .Where(p => p.CanRead && p.CanWrite && !(p.SetMethod.IsVirtual && !p.SetMethod.IsFinal))
                                 .ToArray();
            ColumnProperties[type.TypeHandle] = properties;
            return properties.ToList();
        }

        public static Validator Validate<T>(T entity)
        {
            var infos = new List<ValidInfo>();
            var properties = typeof(T).GetColumnProperties();
            foreach (var property in properties)
            {
                var errors = new List<string>();
                var value = property.GetValue(entity, null);
                var attr = property.GetAttribute<ColumnAttribute>();
                if (attr != null)
                    attr.Validate(value, errors);

                if (errors.Count > 0)
                    infos.Add(new ValidInfo(ValidLevel.Error, property.Name, errors));
            }
            return new Validator(infos);
        }

        public static void FillModel<T>(T entity, dynamic model)
        {
            var properties = typeof(T).GetColumnProperties();
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
