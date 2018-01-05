using Known.Extensions;
using Known.Mapping;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Known.Data
{
    /// <summary>
    /// 数据库栏位信息。
    /// </summary>
    class ColumnInfo
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
        /// <returns></returns>
        public static string GetColumnName(PropertyInfo property)
        {
            var attr = property.GetAttribute<ColumnAttribute>();
            if (attr != null)
                return attr.ColumnName;

            return property.Name;
        }
    }

    /// <summary>
    /// 数据库命令缓存类。
    /// </summary>
    public class CommandCache
    {
        private static readonly ConcurrentDictionary<string, IEnumerable<PropertyInfo>> CachedProperties = new ConcurrentDictionary<string, IEnumerable<PropertyInfo>>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, TableAttribute> TypeTableAttributes = new ConcurrentDictionary<RuntimeTypeHandle, TableAttribute>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<ColumnInfo>> TypeColumnNames = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<ColumnInfo>>();

        /// <summary>
        /// 根据SQL获取缓存的数据库命令。
        /// </summary>
        /// <param name="sql">SQL语句。</param>
        /// <param name="param">语句参数。</param>
        /// <returns></returns>
        public static Command GetCommand(string sql, object param = null)
        {
            var command = new Command(sql);
            if (param == null)
                return command;

            if (!CachedProperties.TryGetValue(sql, out IEnumerable<PropertyInfo> pis))
            {
                pis = param.GetType().GetProperties();
                CachedProperties[sql] = pis;
            }

            foreach (var pi in pis)
            {
                command.AddParameter(pi.Name, pi.GetValue(param));
            }

            return command;
        }

        /// <summary>
        /// 根据实体对象获取保存命令。
        /// </summary>
        /// <typeparam name="T">实体对象类型。</typeparam>
        /// <param name="entity">实体对象。</param>
        /// <returns></returns>
        public static Command GetSaveCommand<T>(T entity) where T : EntityBase
        {
            var type = typeof(T);
            var tableName = GetCachedTableAttribute(type).TableName;
            var columnInfos = GetCachedColumnInfos(type);
            
            if (entity.IsNew)
            {
                var columnNames = columnInfos.Select(c => c.ColumnName);
                var insColumnNames = string.Join(",", columnNames);
                var valColumnNames = string.Join(",@", columnNames);
                var sql = $"insert into {tableName}({insColumnNames}) values(@{valColumnNames})";
                var command = new Command(sql);
                foreach (var item in columnInfos)
                {
                    command.AddParameter(item.ColumnName, item.Property.GetValue(entity));
                }
                return command;
            }
            else
            {
                var setColumnInfos = columnInfos.Where(c => !c.IsKey);
                var keyColumnInfos = columnInfos.Where(c => c.IsKey);
                var setColumnNames = string.Join(",", setColumnInfos.Select(c => $"{c.ColumnName}=@{c.ColumnName}"));
                var keyColumnNames = string.Join(" and ", keyColumnInfos.Select(c => $"{c.ColumnName}=@{c.ColumnName}"));
                var sql = $"update {tableName} set {setColumnNames} where {keyColumnNames}";
                var command = new Command(sql);
                foreach (var item in setColumnInfos)
                {
                    command.AddParameter(item.ColumnName, item.Property.GetValue(entity));
                }
                foreach (var item in keyColumnInfos)
                {
                    command.AddParameter(item.ColumnName, item.Property.GetValue(entity));
                }
                return command;
            }
        }

        /// <summary>
        /// 根据实体对象获取删除命令。
        /// </summary>
        /// <typeparam name="T">实体对象类型。</typeparam>
        /// <param name="entity">实体对象。</param>
        /// <returns></returns>
        public static Command GetDeleteCommand<T>(T entity) where T : EntityBase
        {
            var type = typeof(T);
            var tableName = GetCachedTableAttribute(type).TableName;
            var keyColumnInfos = GetCachedColumnInfos(type).Where(c => c.IsKey).ToList();
            var keyColumnNames = string.Join(" and ", keyColumnInfos.Select(c => $"{c.ColumnName}=@{c.ColumnName}"));
            var command = new Command($"delete from {tableName} where {keyColumnNames}");
            foreach (var item in keyColumnInfos)
            {
                command.AddParameter(item.ColumnName, item.Property.GetValue(entity));
            }
            return command;
        }

        private static IEnumerable<ColumnInfo> GetCachedColumnInfos(Type type)
        {
            if (TypeColumnNames.TryGetValue(type.TypeHandle, out IEnumerable<ColumnInfo> columns))
                return columns;

            var attrTable = GetCachedTableAttribute(type);
            columns = type.GetColumnProperties().Select(p =>
            {
                var columnName = ColumnInfo.GetColumnName(p);
                return new ColumnInfo
                {
                    IsKey = attrTable.PrimaryKeys.Contains(columnName),
                    ColumnName = columnName,
                    Property = p
                };
            });
            TypeColumnNames[type.TypeHandle] = columns;
            return columns;
        }

        private static TableAttribute GetCachedTableAttribute(Type type)
        {
            if (TypeTableAttributes.TryGetValue(type.TypeHandle, out TableAttribute attr))
                return attr;

            var attrs = type.GetCustomAttributes<TableAttribute>().ToList();
            if (attrs != null && attrs.Count > 0)
            {
                attr = attrs[0];
            }
            else
            {
                var name = type.Name + "s";
                if (type.IsInterface && name.StartsWith("I"))
                    name = name.Substring(1);
                attr = new TableAttribute(name, "Id", "");
            }

            TypeTableAttributes[type.TypeHandle] = attr;
            return attr;
        }
    }
}
