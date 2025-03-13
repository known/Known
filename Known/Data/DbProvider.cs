namespace Known.Data;

class DbProvider(Database db)
{
    internal static DbProvider Create(Database db)
    {
        var provider = new DbProvider(db);
        switch (db.DatabaseType)
        {
            case DatabaseType.Access:
                provider = new AccessProvider(db);
                break;
            case DatabaseType.SQLite:
                provider = new SQLiteProvider(db);
                break;
            case DatabaseType.SqlServer:
                provider = new SqlServerProvider(db);
                break;
            case DatabaseType.Oracle:
                provider = new OracleProvider(db);
                break;
            case DatabaseType.MySql:
                provider = new MySqlProvider(db);
                break;
            case DatabaseType.PgSql:
                provider = new PgSqlProvider(db);
                break;
            case DatabaseType.DM:
                provider = new DMProvider(db);
                break;
        }
        return provider;
    }

    private string IdName => FormatName(nameof(EntityBase.Id));
    private string CreateTimeName => FormatName(nameof(EntityBase.CreateTime));

    internal Database Database { get; } = db;
    internal SqlBuilder Sql => new(this);

    public virtual string Prefix => "@";
    public virtual string FormatName(string name) => name;
    public virtual object FormatDate(string date) => date;
    public virtual object FormatBoolean(bool value) => value.ToString();
    public virtual string GetDateSql(string name, bool withTime = true) => $"@{name}";

    public string GetTableName(Type type)
    {
        if (type == null)
            return string.Empty;

        Type entityType = type;
        if (DbConfig.TableNames.TryGetValue(type, out Type value))
            entityType = value;

        var tableName = string.Empty;
        var attrs = entityType.GetCustomAttributes(true);
        foreach (var item in attrs)
        {
            if (item is TableAttribute attr)
            {
                tableName = attr.Name;
                break;
            }
        }
        if (string.IsNullOrWhiteSpace(tableName))
            tableName = entityType.Name;

        return tableName;
    }

    public CommandInfo GetCommand(string sql, PagingCriteria criteria, UserInfo user)
    {
        var info = new CommandInfo(this, sql);
        SetCommand(info, criteria, user);
        return info;
    }

    public void SetCommand(CommandInfo info, PagingCriteria criteria, UserInfo user)
    {
        if (string.IsNullOrWhiteSpace(info.CountSql))
            info.CountSql = $"select count(*) {info.Text.Substring(info.Text.IndexOf("from"))}".Replace("@", Prefix);
        if (string.IsNullOrWhiteSpace(info.StatSql))
            info.StatSql = GetStatSql(info.Text, criteria).Replace("@", Prefix);
        info.PageSql = GetPageSql(info.Text, criteria).Replace("@", Prefix);
        info.Params = criteria.ToParameters(user);
    }

    public CommandInfo GetCountCommand<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
    {
        var sb = Sql.SelectCount().From<T>();
        var paramters = new Dictionary<string, object>();
        if (expression != null)
        {
            var qb = new QueryBuilder<T>(this).Where(expression);
            paramters = qb.Parameters;
            sb.WhereSql(qb.WhereSql);
        }
        var sql = sb.ToSqlString();
        return new CommandInfo(this, typeof(T), sql, paramters);
    }

    public CommandInfo GetSelectCommand<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
    {
        var sb = Sql.SelectAll().From<T>();
        var paramters = new Dictionary<string, object>();
        if (expression != null)
        {
            var qb = new QueryBuilder<T>(this).Where(expression);
            if (!string.IsNullOrWhiteSpace(qb.WhereSql))
            {
                paramters = qb.Parameters;
                sb.WhereSql(qb.WhereSql);
            }
        }
        else
        {
            sb.OrderBy(nameof(EntityBase.CreateTime));
        }
        var sql = sb.ToSqlString();
        return new CommandInfo(this, typeof(T), sql, paramters);
    }

    public CommandInfo GetInsertCommand<T>(T data = default)
    {
        var tableName = GetTableName(typeof(T));
        var cmdParams = DbUtils.ToDictionary(data, true);
        var changes = data == null ? cmdParams : [];
        foreach (var item in cmdParams)
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
        return new CommandInfo(this, typeof(T), sql, changes);
    }

    public CommandInfo GetUpdateCommand<T, TKey>(T data = default) where T : EntityBase<TKey>
    {
        var tableName = GetTableName(typeof(T));
        var cmdParams = DbUtils.ToDictionary(data, true);
        var changes = new Dictionary<string, object>();
        foreach (var item in cmdParams)
        {
            if (data.IsChanged(item.Key, item.Value))
                changes[item.Key] = item.Value;
        }

        var changeKeys = new List<string>();
        foreach (var key in changes.Keys)
        {
            changeKeys.Add($"{FormatName(key)}=@{key}");
        }
        var column = string.Join(",", [.. changeKeys]);
        var sql = $"update {FormatName(tableName)} set {column} where {IdName}=@Id";
        changes["Id"] = data.Id;
        return new CommandInfo(this, typeof(T), sql, changes);
    }

    public CommandInfo GetDeleteCommand<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
    {
        var tableName = GetTableName(typeof(T));
        var sql = $"delete from {FormatName(tableName)}";
        var paramters = new Dictionary<string, object>();
        if (expression != null)
        {
            var qb = new QueryBuilder<T>(this).Where(expression);
            paramters = qb.Parameters;
            sql += $" where {qb.WhereSql}";
        }
        return new CommandInfo(this, typeof(T), sql, paramters);
    }

    internal virtual string GetTableSql(string dbName) => "";
    internal virtual string GetTableScript(string tableName, DbModelInfo info) => "";

    internal static string GetColumnName(string column, int maxLength)
    {
        column ??= "";
        if (column.Length < maxLength)
            column += new string(' ', maxLength - column.Length);

        return column;
    }

    internal virtual string GetTopSql(int size, string text)
    {
        return $"select t.* from (select t1.*,row_number() over (order by 1) row_no from ({text}) t1) t where t.row_no>0 and t.row_no<={size}";
    }

    internal virtual string GetPageSql(string text, string order, PagingCriteria criteria)
    {
        var startNo = criteria.PageSize * (criteria.PageIndex - 1);
        var endNo = startNo + criteria.PageSize;
        return $"select t.* from (select t1.*,row_number() over (order by {order}) row_no from ({text}) t1) t where t.row_no>{startNo} and t.row_no<={endNo}";
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

    private string GetStatSql(string text, PagingCriteria criteria)
    {
        var statisColumns = criteria.StatisticColumns.Select(c =>
        {
            if (!string.IsNullOrWhiteSpace(c.Expression))
                return $"{c.Expression} as {FormatName(c.Id)}";

            return $"{c.Function}({FormatName(c.Id)}) as {FormatName(c.Id)}";
        });
        var columns = string.Join(",", statisColumns);
        return $"select {columns} from ({text}) t";
    }
}