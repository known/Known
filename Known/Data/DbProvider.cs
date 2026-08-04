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

    private string CreateTimeName => FormatName(nameof(EntityBase.CreateTime));

    internal Database Database { get; } = db;
    internal SqlBuilder Sql => new(this);

    public virtual string Prefix => "@";
    public virtual string FormatName(string name) => name;
    public virtual object FormatDate(string date) => date;
    public virtual object FormatBoolean(bool value) => value.ToString();
    public virtual string GetBooleanSql(string field, bool isTrue) => isTrue ? $"{field}='True'" : $"{field}='False'";
    public virtual string GetDateSql(string name, bool withTime = true) => $"@{name}";
    public virtual string GetBackupTableSql(string tableName, string suffix) => $"create table {tableName}{suffix} as select * from {tableName}";

    public string GetTableName(Type type)
    {
        if (type == null)
            return string.Empty;

        Type entityType = type;
        if (DbConfig.TableNames.TryGetValue(type, out Type value))
            entityType = value;

        return entityType.TableName();
    }

    public CommandInfo GetCommand(string sql, PagingCriteria criteria, UserInfo user)
    {
        var info = new CommandInfo(this, sql);
        SetCommand(info, criteria, user);
        return info;
    }

    public void SetCommand(CommandInfo info, PagingCriteria criteria, UserInfo user)
    {
        if (!string.IsNullOrWhiteSpace(info.Text))
        {
            if (DbConfig.OnCountSql != null)
                info.CountSql = DbConfig.OnCountSql.Invoke(info.Text).Replace("@", Prefix);
            if (string.IsNullOrWhiteSpace(info.CountSql))
                info.CountSql = $"select count(*) from ({info.Text}) t".Replace("@", Prefix);
            if (string.IsNullOrWhiteSpace(info.StatSql))
                info.StatSql = GetStatSql(info.Text, criteria).Replace("@", Prefix);
            //if (string.IsNullOrWhiteSpace(info.IdSql))
            //    info.IdSql = GetIdSql(info.Text, criteria).Replace("@", Prefix);
            info.PageSql = GetPageSql(info.Text, criteria).Replace("@", Prefix);
        }
        var parameters = criteria.ToParameters(user);
        info.Parameters = [];
        foreach (var item in parameters)
        {
            info.Parameters.Add(item.Key, item.Value);
        }
    }

    public CommandInfo GetCountCommand<T>(Expression<Func<T, bool>> expression = null, bool applyTenant = true) where T : class, new()
    {
        var sb = Sql.SelectCount().From<T>();
        var paramters = new Dictionary<string, object>();
        var whereSql = string.Empty;
        if (expression != null)
        {
            var qb = new QueryBuilder<T>(this).Where(expression);
            paramters = qb.Parameters;
            whereSql = qb.WhereSql;
        }

        ApplyTenantWhere<T>(ref whereSql, paramters, applyTenant);

        if (!string.IsNullOrWhiteSpace(whereSql))
            sb.WhereSql(whereSql);

        var sql = sb.ToSqlString();
        return new CommandInfo(this, typeof(T), sql, paramters);
    }

    public CommandInfo GetSelectCommand<T>(Expression<Func<T, bool>> expression = null, bool applyTenant = true) where T : class, new()
    {
        var sb = Sql.SelectAll().From<T>();
        var paramters = new Dictionary<string, object>();
        var whereSql = string.Empty;
        if (expression != null)
        {
            var qb = new QueryBuilder<T>(this).Where(expression);
            if (!string.IsNullOrWhiteSpace(qb.WhereSql))
            {
                paramters = qb.Parameters;
                whereSql = qb.WhereSql;
            }
        }

        ApplyTenantWhere<T>(ref whereSql, paramters, applyTenant);

        if (!string.IsNullOrWhiteSpace(whereSql))
            sb.WhereSql(whereSql);
        else
            sb.OrderBy(nameof(EntityBase.CreateTime));

        var sql = sb.ToSqlString();
        return new CommandInfo(this, typeof(T), sql, paramters);
    }

    public CommandInfo GetInsertCommand<T>(T data = default)
    {
        var type = typeof(T);
        var tableName = GetTableName(type);
        var cmdParams = DbUtils.ToDictionary(data);
        var identityFields = Database.DatabaseType == DatabaseType.SqlServer
                           ? TypeCache.Fields(type)
                                      .Where(d => d.IsKey && d.IsAutoKey)
                                      .Select(d => d.Property.GetFieldName())
                                      .ToHashSet(StringComparer.OrdinalIgnoreCase)
                           : null;
        var changes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in cmdParams)
        {
            if (identityFields?.Contains(item.Key) == true)
                continue;

            if (data == null || item.Value != null)
                changes[item.Key] = item.Value;
        }

        var keys = new List<string>();
        foreach (var key in changes.Keys)
        {
            keys.Add(key);
        }
        var cloumn = string.Join(",", [.. keys.Select(FormatName)]);
        var value = string.Join(",", [.. keys.Select(k => $"@{k}")]);
        var sql = $"insert into {FormatName(tableName)}({cloumn}) values({value})";
        return new CommandInfo(this, typeof(T), sql, changes);
    }

    public CommandInfo GetUpdateCommand<T>(T data = default) where T : BaseEntity
    {
        var type = typeof(T);
        var tableName = GetTableName(type);
        var cmdParams = DbUtils.ToDictionary(data);
        var changes = new Dictionary<string, object>();
        var fields = TypeCache.Fields(type);
        foreach (var item in fields)
        {
            var fieldName = item.Property.GetFieldName();
            if (!cmdParams.TryGetValue(fieldName, out var value))
                continue;

            if (data.IsChanged(item.Name, value))
                changes[fieldName] = value;
        }

        var changeKeys = new List<string>();
        foreach (var item in changes.Keys)
        {
            changeKeys.Add($"{FormatName(item)}=@{item}");
        }

        var keys = new List<string>();
        var keyFields = fields.Where(d => d.IsKey).ToList();
        foreach (var item in keyFields)
        {
            var fieldName = item.Property.GetFieldName();
            keys.Add($"{FormatName(fieldName)}=@{fieldName}");
            changes[fieldName] = cmdParams[fieldName];
        }

        var column = string.Join(",", changeKeys);
        var key = string.Join(" and ", keys);
        var sql = $"update {FormatName(tableName)} set {column} where {key}";
        return new CommandInfo(this, typeof(T), sql, changes);
    }

    public CommandInfo GetDeleteCommand<T>(Expression<Func<T, bool>> expression = null) where T : class, new()
    {
        var tableName = GetTableName(typeof(T));
        var sql = $"delete from {FormatName(tableName)}";
        var paramters = new Dictionary<string, object>();
        var whereSql = string.Empty;

        if (expression != null)
        {
            var qb = new QueryBuilder<T>(this).Where(expression);
            paramters = qb.Parameters;
            whereSql = qb.WhereSql;
        }

        if (!string.IsNullOrWhiteSpace(whereSql))
            sql += $" where {whereSql}";

        return new CommandInfo(this, typeof(T), sql, paramters);
    }

    private void ApplyTenantWhere<T>(ref string whereSql, Dictionary<string, object> parameters, bool applyTenant)
    {
        if (!applyTenant)
            return;

        if (!TryGetTenantCompNo(typeof(T), out var compNo))
            return;

        var tenantWhere = $"{FormatName(nameof(EntityBase.CompNo))}=@{nameof(EntityBase.CompNo)}";
        whereSql = string.IsNullOrWhiteSpace(whereSql) ? tenantWhere : $"{whereSql} and {tenantWhere}";
        parameters[nameof(EntityBase.CompNo)] = compNo;
    }

    private bool TryGetTenantCompNo(Type type, out string compNo)
    {
        compNo = null;
        if (!Database.NeedTenantFilter(type))
            return false;

        var user = Database.User;
        compNo = user?.CompNo;
        if (user?.IsChangeTenant == true)
            compNo = user?.TenantNo;

        return !string.IsNullOrWhiteSpace(compNo);
    }

    internal virtual string GetTableSql(string dbName) => "";
    internal virtual string GetTableScript(string tableName, DbModelInfo info) => "";
    internal virtual string GetAddFieldScript(string tableName, List<FieldInfo> fields) => "";

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
        var startNo = criteria.StartIndex > 0 ? criteria.StartIndex : criteria.PageSize * (criteria.PageIndex - 1);
        var endNo = startNo + criteria.PageSize;
        return $"select t.* from (select t1.*,row_number() over (order by {order}) row_no from ({text}) t1) t where t.row_no>{startNo} and t.row_no<={endNo}";
    }

    private string GetPageSql(string text, PagingCriteria criteria)
    {
        var order = GetOrderBy(criteria);
        if (criteria.PageIndex <= 0)
            return $"select t.* from ({text}) t order by {order}";

        return GetPageSql(text, order, criteria);
    }

    private string GetOrderBy(PagingCriteria criteria)
    {
        var order = string.Empty;
        if (criteria.OrderBys != null && criteria.OrderBys.Length > 0)
        {
            var orderBys = new List<string>();
            foreach (var item in criteria.OrderBys)
            {
                if (string.IsNullOrWhiteSpace(item))
                    continue;

                if (item.Contains('('))
                {
                    if (IsSafeOrderBy(item))
                        orderBys.Add(item);
                }
                else if (item.Contains("desc"))
                {
                    var orderBy = GetOrderBy(criteria, item, "desc");
                    if (!string.IsNullOrWhiteSpace(orderBy))
                        orderBys.Add(orderBy);
                }
                else
                {
                    var orderBy = GetOrderBy(criteria, item, "asc");
                    if (!string.IsNullOrWhiteSpace(orderBy))
                        orderBys.Add(orderBy);
                }
            }
            order = string.Join(",", orderBys);
        }

        if (string.IsNullOrWhiteSpace(order))
            order = $"{CreateTimeName} desc";

        return order;
    }

    private string GetOrderBy(PagingCriteria criteria, string item, string sort)
    {
        var field = item.Replace(sort, "", StringComparison.OrdinalIgnoreCase).Trim();
        var prefix = string.Empty;
        var key = field;
        var index = field.LastIndexOf('.');
        if (index >= 0)
        {
            prefix = field[..(index + 1)];
            key = field[(index + 1)..];
        }

        var resolvedKey = criteria.GetFieldName(key);
        // 如果解析后的字段名带表前缀（含 .），而原始字段名不带，
        // 说明该映射来自 Fields 字典（join 查询），
        // 分页包装后表别名不可见，需回退用属性名（SELECT 列别名）
        key = resolvedKey.Contains('.') && !key.Contains('.') ? key : resolvedKey;
        // 校验排序字段名，防止通过OrderBys注入SQL
        if (!IsSafeIdentifier(prefix + key))
            return string.Empty;
        return $"{prefix}{FormatName(key)} {sort}";
    }

    private string GetStatSql(string text, PagingCriteria criteria)
    {
        var statisColumns = criteria.StatisticColumns.Select(c =>
        {
            if (!string.IsNullOrWhiteSpace(c.Expression))
            {
                // 校验统计表达式，防止SQL注入
                if (!IsSafeExpression(c.Expression) || !IsSafeIdentifier(c.Id))
                    return string.Empty;
                return $"{c.Expression} as {FormatName(c.Id)}";
            }

            // 统计函数仅允许内置聚合函数
            if (!IsSafeFunction(c.Function) || !IsSafeIdentifier(c.Id))
                return string.Empty;
            return $"{c.Function}({FormatName(c.Id)}) as {FormatName(c.Id)}";
        });
        var columns = string.Join(",", statisColumns.Where(s => !string.IsNullOrWhiteSpace(s)));
        return $"select {columns} from ({text}) t";
    }

    private static readonly HashSet<string> SafeAggregates = new(StringComparer.OrdinalIgnoreCase)
    {
        "sum", "count", "avg", "min", "max"
    };

    private static bool IsSafeFunction(string function)
    {
        return !string.IsNullOrWhiteSpace(function) && SafeAggregates.Contains(function);
    }

    private static bool IsSafeExpression(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            return false;

        var text = expression.Trim();
        // 屏蔽语句终止符与注释符
        if (text.Contains(';') || text.Contains("--") || text.Contains('#') ||
            text.Contains("/*") || text.Contains("*/"))
            return false;

        // 屏蔽子查询/联合查询等关键字
        var keywords = new[] { "select", "union", "insert", "update", "delete" };
        foreach (var keyword in keywords)
        {
            if (text.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                return false;
        }
        return true;
    }

    private static bool IsSafeIdentifier(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        var parts = name.Split('.');
        if (parts.Length > 2)
            return false;

        foreach (var part in parts)
        {
            if (string.IsNullOrWhiteSpace(part))
                return false;

            if (!char.IsLetter(part[0]) && part[0] != '_')
                return false;

            for (int i = 1; i < part.Length; i++)
            {
                var c = part[i];
                if (!char.IsLetterOrDigit(c) && c != '_')
                    return false;
            }
        }
        return true;
    }

    private static bool IsSafeOrderBy(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        // 仅允许安全的函数排序表达式，如 sum(field)、count(distinct field)
        var trimmed = text.Trim();
        var start = trimmed.IndexOf('(');
        var end = trimmed.LastIndexOf(')');
        if (start <= 0 || end != trimmed.Length - 1)
            return false;

        var function = trimmed[..start];
        var args = trimmed[(start + 1)..end];
        if (!IsSafeIdentifier(function))
            return false;

        foreach (var c in args)
        {
            if (!char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c) &&
                c != '_' && c != '.' && c != ',' && c != '*' &&
                c != '+' && c != '-' && c != '/' && c != '%' && c != ' ')
                return false;
        }
        return true;
    }
}
