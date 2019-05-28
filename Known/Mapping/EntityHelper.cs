using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Known.Extensions;

namespace Known.Mapping
{
    /// <summary>
    /// 实体帮助者类。
    /// </summary>
    public sealed class EntityHelper
    {
        private static readonly ConcurrentDictionary<Type, EntityMapper> EntityMappers = new ConcurrentDictionary<Type, EntityMapper>();
        private static readonly ConcurrentDictionary<Type, List<PropertyInfo>> ColumnProperties = new ConcurrentDictionary<Type, List<PropertyInfo>>();
        private static readonly ConcurrentDictionary<Type, TableAttribute> TypeTables = new ConcurrentDictionary<Type, TableAttribute>();
        private static readonly ConcurrentDictionary<Type, List<ColumnInfo>> TypeColumns = new ConcurrentDictionary<Type, List<ColumnInfo>>();

        /// <summary>
        /// 初始化程序集中所有实体映射器，缓存实体所有数据库栏位信息。
        /// </summary>
        /// <param name="assembly">程序集对象。</param>
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

        /// <summary>
        /// 根据指定的泛型获取数据库表名。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <returns>数据库表名。</returns>
        public static string GetTableName<T>()
        {
            return GetTableName(typeof(T));
        }

        /// <summary>
        /// 根据指定的类型获取数据库表名。
        /// </summary>
        /// <param name="type">实体类型。</param>
        /// <returns>数据库表名。</returns>
        public static string GetTableName(Type type)
        {
            var attr = GetTableAttribute(type);
            return attr != null ? attr.TableName : type.Name;
        }

        /// <summary>
        /// 根据指定的泛型获取实体表属性。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <returns>实体表属性。</returns>
        public static TableAttribute GetTableAttribute<T>()
        {
            return GetTableAttribute(typeof(T));
        }

        /// <summary>
        /// 根据指定的类型获取实体表属性。
        /// </summary>
        /// <param name="type">实体类型。</param>
        /// <returns>实体表属性。</returns>
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

        /// <summary>
        /// 根据指定的泛型获取实体表所有栏位信息。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <returns>栏位信息集合。</returns>
        public static List<ColumnInfo> GetColumnInfos<T>()
        {
            return GetColumnInfos(typeof(T));
        }

        /// <summary>
        /// 根据指定的类型获取实体表所有栏位信息。
        /// </summary>
        /// <param name="type">实体类型。</param>
        /// <returns>栏位信息集合。</returns>
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

        /// <summary>
        /// 根据指定的泛型获取所有数据库栏位属性。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <returns>栏位属性集合。</returns>
        public static List<PropertyInfo> GetColumnProperties<T>()
        {
            return GetColumnProperties(typeof(T));
        }

        /// <summary>
        /// 根据指定的类型获取所有数据库栏位属性。
        /// </summary>
        /// <param name="type">实体类型。</param>
        /// <returns>栏位属性集合。</returns>
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

        /// <summary>
        /// 验证实体所有属性的基本校验。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="entity">实体对象。</param>
        /// <returns>验证结果。</returns>
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

        /// <summary>
        /// 给实体对象填充表单提交的数据。
        /// </summary>
        /// <typeparam name="T">实体类型。</typeparam>
        /// <param name="entity">实体对象。</param>
        /// <param name="model">表单模型。</param>
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
