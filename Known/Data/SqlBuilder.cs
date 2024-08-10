namespace Known.Data;

class SqlBuilder
{
    internal static SqlBuilder Create(DatabaseType type)
    {
        var builder = new SqlBuilder();
        switch (type)
        {
            case DatabaseType.Access:
                builder = new AccessBuilder();
                break;
            case DatabaseType.SQLite:
                builder = new SQLiteBuilder();
                break;
            case DatabaseType.SqlServer:
                builder = new SqlServerBuilder();
                break;
            case DatabaseType.Oracle:
                builder = new OracleBuilder();
                break;
            case DatabaseType.MySql:
                builder = new MySqlBuilder();
                break;
            case DatabaseType.PgSql:
                builder = new PgSqlBuilder();
                break;
        }

        builder.DatabaseType = type;
        return builder;
    }

    private string IdName => FormatName(nameof(EntityBase.Id));
    private string CreateTimeName => FormatName(nameof(EntityBase.CreateTime));

    internal DatabaseType DatabaseType { get; set; }

    public virtual string Prefix => "@";

    public virtual string FormatName(string name) => name;

    public virtual string GetDateSql(string name, bool withTime = true) => $"@{name}";

    protected virtual string GetTopSql(int size, string text)
    {
        return $@"
select t.* from (
    select t1.*,row_number() over (order by 1) row_no 
    from ({text}) t1
) t where t.row_no>0 and t.row_no<={size}";
    }

    protected virtual string GetPageSql(string text, string order, PagingCriteria criteria)
    {
        var startNo = criteria.PageSize * (criteria.PageIndex - 1);
        var endNo = startNo + criteria.PageSize;
        return $@"
select t.* from (
    select t1.*,row_number() over (order by {order}) row_no 
    from ({text}) t1
) t where t.row_no>{startNo} and t.row_no<={endNo}";
    }

    protected virtual string GetSumSql(string text, PagingCriteria criteria)
    {
        var columns = string.Join(",", criteria.SumColumns.Select(c => $"sum({FormatName(c)}) as {FormatName(c)}"));
        return $"select {columns} from ({text}) t";
    }

    public CommandInfo GetCommand(string sql, object param = null) => new(this, sql, param);

    public CommandInfo GetCommand(string sql, PagingCriteria criteria, UserInfo user)
    {
        var info = new CommandInfo(this, sql);
        info.CountSql = $"select count(*) {sql.Substring(sql.IndexOf("from"))}".Replace("@", Prefix);
        info.PageSql = GetPageSql(sql, criteria).Replace("@", Prefix);
        info.SumSql = GetSumSql(sql, criteria).Replace("@", Prefix);
        info.Params = criteria.ToParameters(user);
        return info;
    }

    public CommandInfo GetTopCommand(int size, string sql, object param = null)
    {
        var text = GetTopSql(size, sql);
        return new CommandInfo(this, text, param);
    }

    internal CommandInfo GetSelectCommand<T>(QueryBuilder<T> builder)
    {
        var select = builder.SelectSql;
        if (string.IsNullOrWhiteSpace(select))
            select = "*";
        var sql = $"select {select} from {builder.TableName}";
        if (!string.IsNullOrWhiteSpace(builder.JoinSql))
            sql += builder.JoinSql;
        if (!string.IsNullOrWhiteSpace(builder.WhereSql))
            sql += $" where {builder.WhereSql}";
        if (!string.IsNullOrWhiteSpace(builder.GroupSql))
            sql += $" group by {builder.GroupSql}";
        if (!string.IsNullOrWhiteSpace(builder.OrderSql))
            sql += $" order by {builder.OrderSql}";
        return new CommandInfo(this, sql, builder.Parameters);
    }

    public CommandInfo GetSelectCommand<T>(Expression<Func<T, bool>> expression = null)
    {
        var tableName = GetTableName<T>(true);
        var sql = $"select * from {tableName}";
        var paramters = new Dictionary<string, object>();
        if (expression != null)
        {
            var qb = new QueryBuilder<T>(this).Where(expression);
            if (!string.IsNullOrWhiteSpace(qb.WhereSql))
            {
                paramters = qb.Parameters;
                sql += $" where {qb.WhereSql}";
            }
        }
        else
        {
            sql += $" order by {CreateTimeName}";
        }
        return new CommandInfo(this, sql, paramters);
    }

    public CommandInfo GetSelectCommand<T>(string id)
    {
        var tableName = GetTableName<T>();
        return GetSelectCommand(tableName, id);
    }

    public CommandInfo GetSelectCommand<T>(string[] ids)
    {
        var idTexts = new List<string>();
        var paramters = new Dictionary<string, object>();
        for (int i = 0; i < ids.Length; i++)
        {
            idTexts.Add($"{IdName}=@id{i}");
            paramters.Add($"id{i}", ids[i]);
        }

        var tableName = GetTableName<T>(true);
        var idText = string.Join(" or ", [.. idTexts]);
        var sql = $"select * from {tableName} where {idText}";
        return new CommandInfo(this, sql, paramters);
    }

    public CommandInfo GetSelectCommand(string tableName, string id)
    {
        var sql = $"select * from {FormatName(tableName)} where {IdName}=@id";
        return new CommandInfo(this, sql, new { id });
    }

    internal CommandInfo GetCountCommand<T>(QueryBuilder<T> builder)
    {
        var tableName = GetTableName<T>(true);
        var sql = $"select count(*) from {tableName}";
        if (!string.IsNullOrWhiteSpace(builder.WhereSql))
            sql += $" where {builder.WhereSql}";
        return new CommandInfo(this, sql, builder.Parameters);
    }

    public CommandInfo GetCountCommand<T>(Expression<Func<T, bool>> expression = null)
    {
        var tableName = GetTableName<T>(true);
        var sql = $"select count(*) from {tableName}";
        var paramters = new Dictionary<string, object>();
        if (expression != null)
        {
            var qb = new QueryBuilder<T>(this).Where(expression);
            paramters = qb.Parameters;
            sql += $" where {qb.WhereSql}";
        }
        return new CommandInfo(this, sql, paramters);
    }

    public CommandInfo GetCountCommand(string tableName, string id)
    {
        var sql = $"select count(*) from {FormatName(tableName)} where {IdName}=@id";
        return new CommandInfo(this, sql, new { id });
    }

    public CommandInfo GetInsertCommand(DataTable table)
    {
        var tableName = FormatName(table.TableName);
        var keys = new List<string>();
        foreach (DataColumn item in table.Columns)
        {
            keys.Add(item.ColumnName);
        }
        var cloumn = string.Join(",", keys.Select(FormatName).ToArray());
        var value = string.Join(",", keys.Select(k => $"@{k}").ToArray());
        var sql = $"insert into {tableName}({cloumn}) values({value})";
        return new CommandInfo(this, sql);
    }

    public CommandInfo GetInsertCommand<T>()
    {
        var tableName = GetTableName<T>(true);
        var cmdParams = ToDictionary<T>();
        var keys = new List<string>();
        foreach (var key in cmdParams.Keys)
        {
            keys.Add(key);
        }
        var cloumn = string.Join(",", [.. keys]);
        var value = string.Join(",", keys.Select(k => $"@{k}").ToArray());
        var sql = $"insert into {tableName}({cloumn}) values({value})";
        return new CommandInfo(this, sql);
    }

    public CommandInfo GetInsertCommand<T>(T data)
    {
        var tableName = GetTableName<T>();
        var cmdParams = CommandInfo.MapToDictionary(data);
        return GetInsertCommand(tableName, cmdParams);
    }

    public CommandInfo GetInsertCommand(string tableName, Dictionary<string, object> data)
    {
        var changes = new Dictionary<string, object>();
        foreach (var item in data)
        {
            if (item.Value != null)
                changes[item.Key] = item.Value;
        }

        var keys = new List<string>();
        foreach (var key in changes.Keys)
        {
            keys.Add(key);
        }
        var cloumn = string.Join(",", keys.Select(FormatName).ToArray());
        var value = string.Join(",", keys.Select(k => $"@{k}").ToArray());
        var sql = $"insert into {FormatName(tableName)}({cloumn}) values({value})";
        return new CommandInfo(this, sql, changes);
    }

    public CommandInfo GetUpdateCommand(string tableName, string keyField, Dictionary<string, object> data)
    {
        var changeKeys = new List<string>();
        foreach (var key in data.Keys)
        {
            changeKeys.Add($"{FormatName(key)}=@{key}");
        }
        var column = string.Join(",", [.. changeKeys]);

        var keyFields = new List<string>();
        var keys = keyField.Split(',');
        foreach (var key in keys)
        {
            keyFields.Add($"{FormatName(key)}=@{key}");
        }
        var where = string.Join(" and ", keyFields);
        var sql = $"update {FormatName(tableName)} set {column} where {where}";
        return new CommandInfo(this, sql, data);
    }

    public CommandInfo GetDeleteCommand<T>(Expression<Func<T, bool>> expression = null)
    {
        var tableName = GetTableName<T>(true);
        var sql = $"delete from {tableName}";
        var paramters = new Dictionary<string, object>();
        if (expression != null)
        {
            var qb = new QueryBuilder<T>(this).Where(expression);
            paramters = qb.Parameters;
            sql += $" where {qb.WhereSql}";
        }
        return new CommandInfo(this, sql, paramters);
    }

    public CommandInfo GetDeleteCommand<T>(string id)
    {
        var tableName = GetTableName<T>();
        return GetDeleteCommand(tableName, id);
    }

    public CommandInfo GetDeleteCommand(string tableName, string id)
    {
        var sql = $"delete from {FormatName(tableName)} where {IdName}=@id";
        return new CommandInfo(this, sql, new { id });
    }

    public CommandInfo GetSaveCommand<T>(T entity) where T : EntityBase
    {
        var tableName = GetTableName<T>();
        var cmdParams = ToDictionary(entity);
        if (entity.IsNew)
            return GetInsertCommand(tableName, cmdParams);

        var changes = new Dictionary<string, object>();
        foreach (var item in cmdParams)
        {
            if (entity.IsChanged(item.Key, item.Value))
                changes[item.Key] = item.Value;
        }

        var changeKeys = new List<string>();
        foreach (var key in changes.Keys)
        {
            changeKeys.Add($"{FormatName(key)}=@{key}");
        }
        var column = string.Join(",", [.. changeKeys]);
        var sql = $"update {FormatName(tableName)} set {column} where {IdName}=@Id";
        changes["Id"] = entity.Id;
        return new CommandInfo(this, sql, changes);
    }

    internal string GetTableName<T>(bool format = false) => GetTableName(typeof(T), format);
    internal string GetTableName(Type type, bool format = false)
    {
        var tableName = string.Empty;
        var attrs = type.GetCustomAttributes(true);
        foreach (var item in attrs)
        {
            if (item is TableAttribute attr)
            {
                tableName = attr.Name;
                break;
            }
        }
        if (string.IsNullOrWhiteSpace(tableName))
            tableName = type.Name;

        return format ? FormatName(tableName) : tableName;
    }

    internal string GetColumnName<T>(object field) => GetColumnName(typeof(T), field);
    internal string GetColumnName(Type type, object field)
    {
        var tableName = GetTableName(type, true);
        return GetColumnName(tableName, field);
    }

    internal string GetColumnName(string tableName, object field)
    {
        var name = FormatName($"{field}");
        if (DatabaseType != DatabaseType.PgSql)
            return name;
        return $"{tableName}.{name}";
    }

    private string GetPageSql(string text, PagingCriteria criteria)
    {
        var order = string.Empty;
        if (criteria.OrderBys != null && criteria.OrderBys.Length > 0)
        {
            var orderBys = new List<string>();
            foreach (var item in criteria.OrderBys)
            {
                var fields = item.Split(' ');
                var field = FormatName(fields[0]);
                var sort = fields.Length > 1 ? $" {fields[1]}" : "";
                orderBys.Add($"{field}{sort}");
            }
            order = string.Join(",", orderBys);
        }

        if (string.IsNullOrWhiteSpace(order))
            order = $"{CreateTimeName} desc";

        if (criteria.PageIndex <= 0)
            return $"{text} order by {order}";

        return GetPageSql(text, order, criteria);
    }

    private static Dictionary<string, object> ToDictionary(object value)
    {
        var dic = new Dictionary<string, object>();
        var properties = TypeHelper.Properties(value.GetType());
        foreach (var item in properties)
        {
            if (item.CanRead && item.CanWrite && !item.GetMethod.IsVirtual)
            {
                dic[item.Name] = item.GetValue(value, null);
            }
        }
        return dic;
    }

    private static Dictionary<string, object> ToDictionary<T>()
    {
        var dic = new Dictionary<string, object>();
        var properties = TypeHelper.Properties(typeof(T));
        foreach (var item in properties)
        {
            if (item.CanRead && item.CanWrite && !item.GetMethod.IsVirtual)
            {
                dic[item.Name] = null;
            }
        }
        return dic;
    }
}