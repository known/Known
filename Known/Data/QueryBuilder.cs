namespace Known.Data;

public class QueryBuilder<T> where T : class, new()
{
    private readonly Database db;
    private readonly DbProvider provider;
    private readonly List<string> joins = [];
    private readonly List<string> selects = [];
    private readonly List<string> groupBys = [];
    private readonly List<string> orderBys = [];

    internal QueryBuilder(Database db) : this(db.Provider)
    {
        this.db = db;
        if (db.DatabaseType == DatabaseType.Other)
            throw new SystemException("Not supporting the DatabaseType.Other.");
    }

    internal QueryBuilder(DbProvider provider)
    {
        this.provider = provider;
        TableName = provider.GetTableName<T>();
        orderBys = [];
        WhereSql = string.Empty;
        Parameters = [];
    }

    internal string TableName { get; }
    internal string JoinSql => string.Join(" ", joins);
    internal string SelectSql => string.Join(",", selects);
    internal string WhereSql { get; private set; }
    internal string GroupSql => string.Join(",", groupBys);
    internal string OrderSql => string.Join(",", orderBys);
    internal Dictionary<string, object> Parameters { get; private set; }

    //public QueryBuilder<T> LeftJoin<T1>(Expression<Func<T, T1, bool>> expression)
    //{
    //    var tableName = builder.GetTableName<T1>(true);
    //    var helper = new ExpressionHelper(builder);
    //    var where = helper.RouteExpression(expression);
    //    joins.Add($" left join {tableName} on {where}");
    //    return this;
    //}

    //TODO：优化Select表达式
    internal QueryBuilder<T> Select(params Expression<Func<T, object>>[] selectors)
    {
        if (selectors == null || selectors.Length == 0)
        {
            selects.Add($"{TableName}.*");
        }
        else
        {
            foreach (var selector in selectors)
            {
                selects.Add(GetColumnName(selector));
            }
        }
        return this;
    }

    internal QueryBuilder<T> Select<TItem>(params Expression<Func<TItem, object>>[] selectors)
    {
        if (selectors == null || selectors.Length == 0)
        {
            var tableName = provider.GetTableName<TItem>();
            selects.Add($"{tableName}.*");
        }
        else
        {
            foreach (var selector in selectors)
            {
                selects.Add(GetColumnName(selector));
            }
        }
        return this;
    }

    internal QueryBuilder<T> Select(Expression<Func<T, object>> selector, string asName)
    {
        var name = GetColumnName(selector);
        return AddSelect(name, asName);
    }

    internal QueryBuilder<T> SelectCount(string asName = null) => AddSelect("count(*)", asName);

    public QueryBuilder<T> Where(Expression<Func<T, bool>> expression)
    {
        var helper = new ExpressionHelper(provider);
        helper.RouteExpression(expression);
        WhereSql = helper.WhereSql;
        Parameters = helper.Parameters;
        return this;
    }

    public QueryBuilder<T> GroupBy(params Expression<Func<T, object>>[] selectors)
    {
        foreach (var selector in selectors)
        {
            groupBys.Add(GetColumnName(selector));
        }
        return this;
    }

    public QueryBuilder<T> OrderBy(params Expression<Func<T, object>>[] selectors)
    {
        foreach (var selector in selectors)
        {
            orderBys.Add(GetColumnName(selector));
        }
        return this;
    }

    public QueryBuilder<T> OrderByDescending(params Expression<Func<T, object>>[] selectors)
    {
        foreach (var selector in selectors)
        {
            var name = GetColumnName(selector);
            orderBys.Add($"{name} desc");
        }
        return this;
    }

    public async Task<int> CountAsync()
    {
        var info = provider.GetCountCommand(this);
        return await db.ScalarAsync<int>(info);
    }

    public async Task<bool> ExistsAsync()
    {
        var info = provider.GetCountCommand(this);
        return await db.ScalarAsync<int>(info) > 0;
    }

    public Task<T> FirstAsync() => FirstAsync<T>();
    public Task<TItem> FirstAsync<TItem>()
    {
        var info = provider.GetSelectCommand(this);
        return db.QueryAsync<TItem>(info);
    }

    public Task<List<T>> ToListAsync() => ToListAsync<T>();
    public Task<List<TItem>> ToListAsync<TItem>()
    {
        var info = provider.GetSelectCommand(this);
        return db.QueryListAsync<TItem>(info);
    }

    public Task<PagingResult<T>> ToPageAsync(PagingCriteria criteria) => ToPageAsync<T>(criteria);
    public Task<PagingResult<TItem>> ToPageAsync<TItem>(PagingCriteria criteria) where TItem : class, new()
    {
        var info = provider.GetSelectCommand(this);
        criteria.CmdParams = info.Params;
        return db.QueryPageAsync<TItem>(info.Text, criteria);
    }

    public CommandInfo ToCommand() => provider.GetSelectCommand(this);

    private string GetColumnName(Expression<Func<T, object>> selector)
    {
        var pi = TypeHelper.Property(selector);
        return provider.GetColumnName(TableName, pi.Name);
    }

    private string GetColumnName<TItem>(Expression<Func<TItem, object>> selector)
    {
        var pi = TypeHelper.Property(selector);
        return provider.GetColumnName<TItem>(pi.Name);
    }

    private QueryBuilder<T> AddSelect(string name, string asName = null)
    {
        var select = name;
        if (!string.IsNullOrWhiteSpace(asName))
        {
            var nameAs = provider.FormatName(asName);
            select += $" as {nameAs}";
        }
        selects.Add(select);
        return this;
    }
}