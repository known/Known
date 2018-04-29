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
        /// <returns>数据库命令。</returns>
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

        /// <summary>
        /// 根据表名及参数获取查询数据命令。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="parameters">命令参数字典。</param>
        /// <returns>数据库命令。</returns>
        public static Command GetSelectCommand(string tableName, Dictionary<string, object> parameters = null)
        {
            var whereSql = string.Empty;
            if (parameters != null && parameters.Count > 0)
            {
                whereSql = " where " + string.Join(" and ", parameters.Keys.Select(k => string.Format("{0}=@{0}", k)));
            }
            var text = string.Format("select * from {0}{1}", tableName, whereSql);
            return new Command(text, parameters);
        }

        /// <summary>
        /// 根据数据表获取插入数据命令。
        /// </summary>
        /// <param name="table">数据表。</param>
        /// <returns>数据库命令。</returns>
        public static Command GetInsertCommand(DataTable table)
        {
            var columns = table.Columns.OfType<DataColumn>().Select(c => c.ColumnName);
            var columnSql = string.Join(",", columns.Select(k => k));
            var valueSql = string.Join(",", columns.Select(k => string.Format("@{0}", k)));
            var text = string.Format("insert into {0}({1}) values({2})", table.TableName, columnSql, valueSql);
            return new Command(text);
        }

        /// <summary>
        /// 根据表名及参数获取插入数据命令。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="parameters">命令参数字典。</param>
        /// <returns>数据库命令。</returns>
        public static Command GetInsertCommand(string tableName, Dictionary<string, object> parameters)
        {
            var columnSql = string.Join(",", parameters.Keys.Select(k => k));
            var valueSql = string.Join(",", parameters.Keys.Select(k => string.Format("@{0}", k)));
            var text = string.Format("insert into {0}({1}) values({2})", tableName, columnSql, valueSql);
            return new Command(text, parameters);
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
            var columnSql = string.Join(",", parameters.Keys.Where(k => !keyFields.Contains(k)).Select(k => string.Format("{0}=@{0}", k)));
            var whereSql = string.Join(" and ", keyFields.Split(',').Select(k => string.Format("{0}=@{0}", k)));
            var text = string.Format("update {0} set {1} where {2}", tableName, columnSql, whereSql);
            return new Command(text, parameters);
        }

        /// <summary>
        /// 根据表名及参数获取删除数据命令。
        /// </summary>
        /// <param name="tableName">表名。</param>
        /// <param name="parameters">命令参数字典。</param>
        /// <returns>数据库命令。</returns>
        public static Command GetDeleteCommand(string tableName, Dictionary<string, object> parameters)
        {
            var whereSql = string.Format(" and ", parameters.Keys.Select(k => string.Format("{0}=@{0}", k)));
            var text = string.Format("delete from {0} where {1}", tableName, whereSql);
            return new Command(text, parameters);
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
