namespace Known.Data;

/// <summary>
/// 查询建造者类。
/// </summary>
/// <typeparam name="T"></typeparam>
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

    /// <summary>
    /// 获取Where表达式语句。
    /// </summary>
    /// <param name="expression">条件表达式。</param>
    /// <returns>查询建造者。</returns>
    public QueryBuilder<T> Where(Expression<Func<T, bool>> expression)
    {
        var helper = new ExpressionHelper(provider);
        helper.RouteExpression(expression);
        WhereSql = helper.WhereSql;
        Parameters = helper.Parameters;
        return this;
    }

    /// <summary>
    /// 获取Group By语句表达式。
    /// </summary>
    /// <param name="selectors">Group By字段属性选择表达式。</param>
    /// <returns>查询建造者。</returns>
    public QueryBuilder<T> GroupBy(params Expression<Func<T, object>>[] selectors)
    {
        foreach (var selector in selectors)
        {
            groupBys.Add(GetColumnName(selector));
        }
        return this;
    }

    /// <summary>
    /// 获取Order By升序语句表达式。
    /// </summary>
    /// <param name="selectors">Order By字段属性选择表达式。</param>
    /// <returns>查询建造者。</returns>
    public QueryBuilder<T> OrderBy(params Expression<Func<T, object>>[] selectors)
    {
        foreach (var selector in selectors)
        {
            orderBys.Add(GetColumnName(selector));
        }
        return this;
    }

    /// <summary>
    /// 获取Order By降序语句表达式。
    /// </summary>
    /// <param name="selectors">Order By字段属性选择表达式。</param>
    /// <returns>查询建造者。</returns>
    public QueryBuilder<T> OrderByDescending(params Expression<Func<T, object>>[] selectors)
    {
        foreach (var selector in selectors)
        {
            var name = GetColumnName(selector);
            orderBys.Add($"{name} desc");
        }
        return this;
    }

    /// <summary>
    /// 异步查询建造者构建的数据数量。
    /// </summary>
    /// <returns>数据数量。</returns>
    public async Task<int> CountAsync()
    {
        var info = provider.GetCountCommand(this);
        return await db.ScalarAsync<int>(info);
    }

    /// <summary>
    /// 异步判断建造者构建的数据是否存在。
    /// </summary>
    /// <returns>是否存在。</returns>
    public async Task<bool> ExistsAsync()
    {
        var info = provider.GetCountCommand(this);
        return await db.ScalarAsync<int>(info) > 0;
    }

    /// <summary>
    /// 异步查询建造者构建的单条数据。
    /// </summary>
    /// <returns>单条数据。</returns>
    public Task<T> FirstAsync() => FirstAsync<T>();

    /// <summary>
    /// 异步查询建造者构建的单条数据。
    /// </summary>
    /// <typeparam name="TItem">泛型类型。</typeparam>
    /// <returns>单条数据。</returns>
    public Task<TItem> FirstAsync<TItem>()
    {
        var info = provider.GetSelectCommand(this, true);
        return db.QueryAsync<TItem>(info);
    }

    /// <summary>
    /// 异步查询建造者构建的多条数据。
    /// </summary>
    /// <returns>多条数据。</returns>
    public Task<List<T>> ToListAsync() => ToListAsync<T>();

    /// <summary>
    /// 异步查询建造者构建的多条数据。
    /// </summary>
    /// <typeparam name="TItem">泛型类型。</typeparam>
    /// <returns>多条数据。</returns>
    public Task<List<TItem>> ToListAsync<TItem>()
    {
        var info = provider.GetSelectCommand(this);
        return db.QueryListAsync<TItem>(info);
    }

    /// <summary>
    /// 异步查询建造者构建的分页数据。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页数据。</returns>
    public Task<PagingResult<T>> ToPageAsync(PagingCriteria criteria) => ToPageAsync<T>(criteria);

    /// <summary>
    /// 异步查询建造者构建的分页数据。
    /// </summary>
    /// <typeparam name="TItem">泛型类型。</typeparam>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页数据。</returns>
    public Task<PagingResult<TItem>> ToPageAsync<TItem>(PagingCriteria criteria) where TItem : class, new()
    {
        var info = provider.GetSelectCommand(this);
        criteria.CmdParams = info.Params;
        return db.QueryPageAsync<TItem>(info.Text, criteria);
    }

    /// <summary>
    /// 将查询建造者转成数据库访问命令信息对象。
    /// </summary>
    /// <returns>数据库访问命令信息对象。</returns>
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