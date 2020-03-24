using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Known.Mapping;

namespace Known.Data
{
    sealed class CommandHelper
    {
        private static readonly ConcurrentDictionary<string, IEnumerable<PropertyInfo>> CachedProperties = new ConcurrentDictionary<string, IEnumerable<PropertyInfo>>();

        public static string GetQueryByIdSql<T>()
        {
            var tableName = EntityHelper.GetTableName<T>();
            return $"select * from {tableName} where id=@id";
        }

        public static string GetQueryListSql<T>()
        {
            var tableName = EntityHelper.GetTableName<T>();
            return $"select * from {tableName}";
        }

        public static string GetQueryListByIdSql<T>(string[] ids)
        {
            var tableName = EntityHelper.GetTableName<T>();
            var id = string.Join("','", ids);
            return $"select * from {tableName} where id in ('{id}')";
        }

        public static string GetCountSql(string sql)
        {
            return $"select count(1) from ({sql}) t";
        }

        public static string GetPagingSql(string providerName, string sql, PagingCriteria criteria)
        {
            var orderBy = string.Join(",", criteria.OrderBys.Select(f => string.Format("t1.{0}", f)));
            var startNo = criteria.PageSize * criteria.PageIndex;
            var endNo = startNo + criteria.PageSize;

            if (string.IsNullOrWhiteSpace(orderBy))
            {
                orderBy = "t1.create_time";
            }

            if (providerName.Contains("MySql"))
            {
                return $@"
select t1.* from (
    {sql}
) t1 
order by {orderBy} 
limit {startNo}, {endNo}";
            }

            return $@"
select t.* from (
    select t1.*,row_number() over (order by {orderBy}) row_no 
    from (
        {sql}
    ) t1
) t where t.row_no>{startNo} and t.row_no<={endNo}";
        }

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

        public static Command GetQueryCommand(string sql, dynamic param = null)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return null;

            var command = new Command(sql);
            if (param == null)
                return command;

            var pis = param.Properties();
            foreach (var pi in pis)
            {
                var value = (string)pi.Value.Value;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    command.AddParameter(pi.Name, value);
                }
            }

            return command;
        }

        public static Command GetSaveCommand<T>(T entity) where T : EntityBase
        {
            if (entity == null)
                return null;

            var tableName = EntityHelper.GetTableName<T>();
            var columns = EntityHelper.GetColumnInfos<T>();

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
                var setColumns = columns.Where(c => !c.IsKey && c.ColumnName != "create_by" && c.ColumnName != "create_time");
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

        public static Command GetDeleteCommand<T>(T entity) where T : EntityBase
        {
            if (entity == null)
                return null;

            var tableName = EntityHelper.GetTableName<T>();
            var keyColumns = EntityHelper.GetColumnInfos<T>().Where(c => c.IsKey).ToList();
            var where = string.Join(" and ", keyColumns.Select(c => $"{c.ColumnName}=@{c.ColumnName}"));
            var command = new Command($"delete from {tableName} where {where}");
            foreach (var item in keyColumns)
            {
                command.AddParameter(item.ColumnName, item.Property.GetValue(entity));
            }
            return command;
        }

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

        public static Command GetDeleteCommand(string tableName, Dictionary<string, object> parameters)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                return null;

            if (parameters == null || parameters.Count == 0)
                return null;

            var where = string.Join(" and ", parameters.Keys.Select(k => $"{k}=@{k}"));
            return new Command($"delete from {tableName} where {where}", parameters);
        }
    }
}
