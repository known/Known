using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Known.Extensions;
using Known.Mapping;

namespace Known.Data
{
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
        /// <returns>数据库命令。</returns>
        public static Command GetCommand(string sql, object param = null)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return null;

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
        /// <returns>数据库命令。</returns>
        public static Command GetSaveCommand<T>(T entity) where T : EntityBase
        {
            if (entity == null)
                return null;

            var type = typeof(T);
            var tableName = GetCachedTableAttribute(type).TableName;
            var columns = GetCachedColumnInfos(type);

            if (entity.IsNew)
            {
                var columnNames = columns.Select(c => c.ColumnName);
                var insColNames = string.Join(",", columnNames);
                var valColNames = string.Join(",@", columnNames);
                var command = new Command($"insert into {tableName}({insColNames}) values(@{valColNames})");
                foreach (var item in columns)
                {
                    command.AddParameter(item.ColumnName, item.Property.GetValue(entity));
                }
                return command;
            }
            else
            {
                var setColumns = columns.Where(c => !c.IsKey);
                var keyColumns = columns.Where(c => c.IsKey);
                var setColNames = string.Join(",", setColumns.Select(c => $"{c.ColumnName}=@{c.ColumnName}"));
                var where = string.Join(" and ", keyColumns.Select(c => $"{c.ColumnName}=@{c.ColumnName}"));
                var command = new Command($"update {tableName} set {setColNames} where {where}");
                foreach (var item in setColumns)
                {
                    command.AddParameter(item.ColumnName, item.Property.GetValue(entity));
                }
                foreach (var item in keyColumns)
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
        /// <returns>数据库命令。</returns>
        public static Command GetDeleteCommand<T>(T entity) where T : EntityBase
        {
            if (entity == null)
                return null;

            var type = typeof(T);
            var tableName = GetCachedTableAttribute(type).TableName;
            var keyColumns = GetCachedColumnInfos(type).Where(c => c.IsKey).ToList();
            var where = string.Join(" and ", keyColumns.Select(c => $"{c.ColumnName}=@{c.ColumnName}"));
            var command = new Command($"delete from {tableName} where {where}");
            foreach (var item in keyColumns)
            {
                command.AddParameter(item.ColumnName, item.Property.GetValue(entity));
            }
            return command;
        }

        /// <summary>
        /// 根据表名及参数获取查询数据命令。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="parameters">命令参数字典。</param>
        /// <returns>数据库命令。</returns>
        public static Command GetSelectCommand(string tableName, Dictionary<string, object> parameters = null)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                return null;

            var where = string.Empty;
            if (parameters != null && parameters.Count > 0)
            {
                where = " where " + string.Join(" and ", parameters.Keys.Select(k => $"{k}=@{k}"));
            }
            return new Command($"select * from {tableName}{where}", parameters);
        }

        /// <summary>
        /// 根据数据表获取插入数据命令。
        /// </summary>
        /// <param name="table">数据表。</param>
        /// <returns>数据库命令。</returns>
        public static Command GetInsertCommand(DataTable table)
        {
            if (table == null || table.Columns.Count == 0)
                return null;

            if (string.IsNullOrWhiteSpace(table.TableName))
                return null;

            var columnNames = table.Columns.OfType<DataColumn>().Select(c => c.ColumnName);
            var columns = string.Join(",", columnNames.Select(k => k));
            var values = string.Join(",", columnNames.Select(k => $"@{k}"));
            return new Command($"insert into {table.TableName}({columns}) values({values})");
        }

        /// <summary>
        /// 根据表名及参数获取插入数据命令。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="parameters">命令参数字典。</param>
        /// <returns>数据库命令。</returns>
        public static Command GetInsertCommand(string tableName, Dictionary<string, object> parameters)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                return null;

            if (parameters == null || parameters.Count == 0)
                return null;

            var columns = string.Join(",", parameters.Keys.Select(k => k));
            var values = string.Join(",", parameters.Keys.Select(k => string.Format("@{0}", k)));
            return new Command($"insert into {tableName}({columns}) values({values})", parameters);
        }

        /// <summary>
        /// 根据表名及参数获取修改数据命令。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="keyFields">主键字段名，多个用“,”分割。</param>
        /// <param name="parameters">命令参数字典。</param>
        /// <returns>数据库命令。</returns>
        public static Command GetUpdateCommand(string tableName, string keyFields, Dictionary<string, object> parameters)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                return null;

            if (string.IsNullOrWhiteSpace(keyFields))
                return null;

            if (parameters == null || parameters.Count == 0)
                return null;

            var columns = string.Join(",", parameters.Keys.Where(k => !keyFields.Contains(k)).Select(k => $"{k}=@{k}"));
            var where = string.Join(" and ", keyFields.Split(',').Select(k => $"{k}=@{k}"));
            return new Command($"update {tableName} set {columns} where {where}", parameters);
        }

        /// <summary>
        /// 根据表名及参数获取删除数据命令。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="parameters">命令参数字典。</param>
        /// <returns>数据库命令。</returns>
        public static Command GetDeleteCommand(string tableName, Dictionary<string, object> parameters)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                return null;

            if (parameters == null || parameters.Count == 0)
                return null;

            var where = string.Join(" and ", parameters.Keys.Select(k => $"{k}=@{k}"));
            return new Command($"delete from {tableName} where {where}", parameters);
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
                attr = new TableAttribute(name, "Id", "");
            }

            TypeTableAttributes[type.TypeHandle] = attr;
            return attr;
        }
    }
}
