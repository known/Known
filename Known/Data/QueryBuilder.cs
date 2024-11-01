namespace Known.Data;

/// <summary>
/// 查询建造者接口。
/// </summary>
/// <typeparam name="T">数据类型。</typeparam>
public interface IQueryBuilder<T> where T : class, new()
{
    /// <summary>
    /// 取得Where SQL语句。
    /// </summary>
    string WhereSql { get; }

    /// <summary>
    /// 取得Where参数字典。
    /// </summary>
    Dictionary<string, object> Parameters { get; }

    /// <summary>
    /// 获取Select字段语句表达式。
    /// </summary>
    /// <typeparam name="TItem">返回类型。</typeparam>
    /// <param name="selector">返回字段选择器。</param>
    /// <returns>查询建造者。</returns>
    IQueryBuilder<TItem> Select<TItem>(Expression<Func<T, TItem>> selector) where TItem : class, new();

    /// <summary>
    /// 获取Where语句表达式。
    /// </summary>
    /// <param name="predicate">条件表达式。</param>
    /// <returns>查询建造者。</returns>
    IQueryBuilder<T> Where(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// 获取Group By语句表达式。
    /// </summary>
    /// <param name="selector">Group By字段属性选择表达式。</param>
    /// <returns>查询建造者。</returns>
    IQueryBuilder<T> GroupBy(Expression<Func<T, object>> selector);

    /// <summary>
    /// 获取Order By升序语句表达式。
    /// </summary>
    /// <param name="selector">Order By字段属性选择表达式。</param>
    /// <returns>查询建造者。</returns>
    IQueryBuilder<T> OrderBy(Expression<Func<T, object>> selector);

    /// <summary>
    /// 获取连续Order By升序语句表达式。
    /// </summary>
    /// <param name="selector">Order By字段属性选择表达式。</param>
    /// <returns>查询建造者。</returns>
    IQueryBuilder<T> ThenBy(Expression<Func<T, object>> selector);

    /// <summary>
    /// 获取Order By降序语句表达式。
    /// </summary>
    /// <param name="selector">Order By字段属性选择表达式。</param>
    /// <returns>查询建造者。</returns>
    IQueryBuilder<T> OrderByDescending(Expression<Func<T, object>> selector);

    /// <summary>
    /// 获取连续Order By降序语句表达式。
    /// </summary>
    /// <param name="selector">Order By字段属性选择表达式。</param>
    /// <returns>查询建造者。</returns>
    IQueryBuilder<T> ThenByDescending(Expression<Func<T, object>> selector);

    /// <summary>
    /// 异步查询建造者构建的数据数量。
    /// </summary>
    /// <returns>数据数量。</returns>
    Task<int> CountAsync();

    /// <summary>
    /// 异步判断建造者构建的数据是否存在。
    /// </summary>
    /// <returns>是否存在。</returns>
    Task<bool> ExistsAsync();

    /// <summary>
    /// 异步查询建造者构建的单条数据。
    /// </summary>
    /// <returns>单条数据。</returns>
    Task<T> FirstAsync();

    /// <summary>
    /// 异步查询建造者构建的单条数据。
    /// </summary>
    /// <typeparam name="TItem">泛型类型。</typeparam>
    /// <returns>单条数据。</returns>
    Task<TItem> FirstAsync<TItem>();

    /// <summary>
    /// 异步查询建造者构建的多条数据。
    /// </summary>
    /// <returns>多条数据。</returns>
    Task<List<T>> ToListAsync();

    /// <summary>
    /// 异步查询建造者构建的多条数据。
    /// </summary>
    /// <typeparam name="TItem">泛型类型。</typeparam>
    /// <returns>多条数据。</returns>
    Task<List<TItem>> ToListAsync<TItem>();
}

class QueryBuilder<T> : IQueryBuilder<T> where T : class, new()
{
    private readonly Database db;
    private readonly DbProvider provider;
    internal List<string> joins = [];
    internal List<string> selects = [];
    internal List<string> groupBys = [];
    internal List<string> orderBys = [];

    internal QueryBuilder(Database db) : this(db.Provider)
    {
        this.db = db;
        if (db.DatabaseType == DatabaseType.Other)
            throw new SystemException("Not supporting the DatabaseType.Other.");
    }

    internal QueryBuilder(DbProvider provider)
    {
        this.provider = provider;
        TableName = provider.GetTableName(typeof(T));
        WhereSql = string.Empty;
        Parameters = [];
    }

    internal string TableName { get; private set; }
    internal string JoinSql => string.Join(" ", joins);
    internal string SelectSql => string.Join(",", selects);
    internal string GroupSql => string.Join(",", groupBys);
    internal string OrderSql => string.Join(",", orderBys);

    public string WhereSql { get; private set; }
    public Dictionary<string, object> Parameters { get; private set; }

    public IQueryBuilder<TItem> Select<TItem>(Expression<Func<T, TItem>> selector) where TItem : class, new()
    {
        var builder = new QueryBuilder<TItem>(db);
        var helper = new ExpressionHelper(provider);
        selects = helper.RouteExpression<TItem>(selector.Body) as List<string>;
        builder.SetBuilder(this);
        return builder;
    }

    public IQueryBuilder<T> Where(Expression<Func<T, bool>> predicate)
    {
        var helper = new ExpressionHelper(provider);
        helper.RouteExpression(predicate);
        WhereSql = helper.WhereSql;
        Parameters = helper.Parameters;
        return this;
    }

    public IQueryBuilder<T> GroupBy(Expression<Func<T, object>> selector)
    {
        groupBys.Add(GetColumnName(selector));
        return this;
    }

    public IQueryBuilder<T> OrderBy(Expression<Func<T, object>> selector)
    {
        orderBys.Add(GetColumnName(selector));
        return this;
    }

    public IQueryBuilder<T> ThenBy(Expression<Func<T, object>> selector) => OrderBy(selector);

    public IQueryBuilder<T> OrderByDescending(Expression<Func<T, object>> selector)
    {
        var name = GetColumnName(selector);
        orderBys.Add($"{name} desc");
        return this;
    }

    public IQueryBuilder<T> ThenByDescending(Expression<Func<T, object>> selector) => OrderByDescending(selector);

    public async Task<int> CountAsync()
    {
        var info = GetCountCommand();
        return await db.ScalarAsync<int>(info);
    }

    public async Task<bool> ExistsAsync() => await CountAsync() > 0;
    public Task<T> FirstAsync() => FirstAsync<T>();

    public Task<TItem> FirstAsync<TItem>()
    {
        var info = GetSelectCommand(true);
        return db.QueryAsync<TItem>(info);
    }

    public Task<List<T>> ToListAsync() => ToListAsync<T>();

    public Task<List<TItem>> ToListAsync<TItem>()
    {
        var info = GetSelectCommand();
        return db.QueryListAsync<TItem>(info);
    }

    internal void SetBuilder<TItem>(QueryBuilder<TItem> builder) where TItem : class, new()
    {
        TableName = builder.TableName;
        joins = builder.joins;
        selects = builder.selects;
        groupBys = builder.groupBys;
        orderBys = builder.orderBys;
        WhereSql = builder.WhereSql;
        Parameters = builder.Parameters;
    }

    private string GetColumnName(Expression<Func<T, object>> selector)
    {
        var pi = TypeHelper.Property(selector);
        return provider.FormatName(pi.Name);
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

    private CommandInfo GetCountCommand()
    {
        var tableName = provider.GetTableName(typeof(T));
        var sql = $"select count(*) from {provider.FormatName(tableName)}";
        if (!string.IsNullOrWhiteSpace(WhereSql))
            sql += $" where {WhereSql}";
        return new CommandInfo(provider, sql, Parameters);
    }

    private CommandInfo GetSelectCommand(bool first = false)
    {
        var select = SelectSql;
        if (string.IsNullOrWhiteSpace(select))
            select = "*";
        var sql = $"select {select} from {provider.FormatName(TableName)}";
        if (!string.IsNullOrWhiteSpace(JoinSql))
            sql += JoinSql;
        if (!string.IsNullOrWhiteSpace(WhereSql))
            sql += $" where {WhereSql}";
        if (!string.IsNullOrWhiteSpace(GroupSql))
            sql += $" group by {GroupSql}";
        if (!string.IsNullOrWhiteSpace(OrderSql))
            sql += $" order by {OrderSql}";

        if (first)
            sql = provider.GetTopSql(1, sql);

        return new CommandInfo(provider, sql, Parameters);
    }
}