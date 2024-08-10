namespace Known.Data;

public class QueryBuilder<T>
{
    private readonly Database db;
    private readonly SqlBuilder builder;
    private readonly List<string> joins = [];
    private readonly List<string> selects = [];
    private readonly List<string> groupBys = [];
    private readonly List<string> orderBys = [];

    internal QueryBuilder(Database db) : this(db.Builder)
    {
        this.db = db;
    }

    internal QueryBuilder(SqlBuilder builder)
    {
        this.builder = builder;
        TableName = builder.GetTableName<T>(true);
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

    public QueryBuilder<T> LeftJoin<T1>(Expression<Func<T, T1, bool>> expression)
    {
        var tableName = builder.GetTableName<T1>(true);
        var helper = new ExpressionHelper(builder);
        var where = helper.RouteExpression(expression);
        joins.Add($" left join {tableName} on {where}");
        return this;
    }

    public QueryBuilder<T> Select(params Expression<Func<T, object>>[] selectors)
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

    public QueryBuilder<T> Select<TItem>(params Expression<Func<TItem, object>>[] selectors)
    {
        if (selectors == null || selectors.Length == 0)
        {
            var tableName = builder.GetTableName<TItem>(true);
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

    public QueryBuilder<T> Select(Expression<Func<T, object>> selector, string nameAs)
    {
        var name = GetColumnName(selector);
        return AddSelect(name, nameAs);
    }

    public QueryBuilder<T> Select<TItem>(Expression<Func<TItem, object>> selector, string nameAs)
    {
        var name = GetColumnName(selector);
        return AddSelect(name, nameAs);
    }

    public QueryBuilder<T> SelectCount(string nameAs = null)
    {
        if (string.IsNullOrWhiteSpace(nameAs))
            selects.Add("count(*)");
        else
            AddSelect("count(*)", nameAs);
        return this;
    }

    private QueryBuilder<T> AddSelect(string name, string nameAs)
    {
        var asName = builder.FormatName(nameAs);
        selects.Add($"{name} as {asName}");
        return this;
    }

    public QueryBuilder<T> Where(Expression<Func<T, bool>> expression)
    {
        var helper = new ExpressionHelper(builder);
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
        var info = builder.GetCountCommand(this);
        return await db.ScalarAsync<int>(info);
    }

    public async Task<bool> ExistsAsync()
    {
        var info = builder.GetCountCommand(this);
        return await db.ScalarAsync<int>(info) > 0;
    }

    public Task<T> FirstAsync() => FirstAsync<T>();
    public Task<TItem> FirstAsync<TItem>()
    {
        var info = builder.GetSelectCommand(this);
        return db.QueryAsync<TItem>(info);
    }

    public Task<List<T>> ToListAsync() => ToListAsync<T>();
    public Task<List<TItem>> ToListAsync<TItem>()
    {
        var info = builder.GetSelectCommand(this);
        return db.QueryListAsync<TItem>(info);
    }

    public Task<PagingResult<T>> ToPageAsync(PagingCriteria criteria) => ToPageAsync<T>(criteria);
    public Task<PagingResult<TItem>> ToPageAsync<TItem>(PagingCriteria criteria)
    {
        var info = builder.GetSelectCommand(this);
        criteria.CmdParams = info.Params;
        return db.QueryPageAsync<TItem>(info.Text, criteria);
    }

    private string GetColumnName(Expression<Func<T, object>> selector)
    {
        var pi = TypeHelper.Property(selector);
        return builder.GetColumnName(TableName, pi.Name);
    }

    private string GetColumnName<TItem>(Expression<Func<TItem, object>> selector)
    {
        var pi = TypeHelper.Property(selector);
        return builder.GetColumnName<TItem>(pi.Name);
    }
}