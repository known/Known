namespace Known.Data;

public partial class Database
{
    /// <summary>
    /// 创建一个查询建造者实例。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <returns>查询建造者实例。</returns>
    public virtual IQueryBuilder<T> Query<T>() where T : class, new() => new QueryBuilder<T>(this);

    /// <summary>
    /// 异步查询单条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="sql">查询SQL语句。</param>
    /// <param name="param">查询参数。</param>
    /// <returns>单条数据。</returns>
    public virtual Task<T> QueryAsync<T>(string sql, object param = null) where T : new()
    {
        var info = new CommandInfo(Provider, typeof(T), sql, param);
        return QueryAsync<T>(info);
    }

    /// <summary>
    /// 异步查询单条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="expression">查询表达式。</param>
    /// <returns>单条数据。</returns>
    public virtual Task<T> QueryAsync<T>(Expression<Func<T, bool>> expression) where T : class, new()
    {
        var info = Provider?.GetSelectCommand(expression);
        return QueryAsync<T>(info);
    }

    /// <summary>
    /// 异步查询实体对象。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <typeparam name="TKey">ID字段类型。</typeparam>
    /// <param name="id">ID字段值。</param>
    /// <returns>实体对象。</returns>
    public virtual Task<T> QueryByIdAsync<T, TKey>(TKey id) where T : EntityBase<TKey>, new()
    {
        return QueryAsync<T>(d => d.Id.Equals(id));
    }

    /// <summary>
    /// 异步查询实体对象。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <param name="id">ID字段值。</param>
    /// <returns>实体对象。</returns>
    public Task<T> QueryByIdAsync<T>(string id) where T : EntityBase<string>, new()
    {
        if (string.IsNullOrWhiteSpace(id))
            return Task.FromResult(default(T));

        return QueryByIdAsync<T, string>(id);
    }

    /// <summary>
    /// 异步查询实体对象。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <param name="id">ID字段值。</param>
    /// <returns>实体对象。</returns>
    public Task<T> QueryByIdAsync<T>(long id) where T : EntityBase<long>, new()
    {
        if (id <= 0)
            return Task.FromResult(default(T));

        return QueryByIdAsync<T, long>(id);
    }

    /// <summary>
    /// 异步查询实体对象。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <param name="id">ID字段值。</param>
    /// <returns>实体对象。</returns>
    public Task<T> QueryByIdAsync<T>(int id) where T : EntityBase<int>, new()
    {
        if (id <= 0)
            return Task.FromResult(default(T));

        return QueryByIdAsync<T, int>(id);
    }

    /// <summary>
    /// 异步查询多条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <returns>多条数据。</returns>
    public virtual Task<List<T>> QueryListAsync<T>() where T : class, new()
    {
        var info = Provider?.GetSelectCommand<T>();
        return QueryListAsync<T>(info);
    }

    /// <summary>
    /// 异步查询多条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="sql">查询SQL语句。</param>
    /// <param name="param">查询参数。</param>
    /// <returns>多条数据。</returns>
    public virtual Task<List<T>> QueryListAsync<T>(string sql, object param = null) where T : new()
    {
        var info = new CommandInfo(Provider, typeof(T), sql, param);
        return QueryListAsync<T>(info);
    }

    /// <summary>
    /// 异步查询多条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="expression">查询表达式。</param>
    /// <returns>多条数据。</returns>
    public virtual Task<List<T>> QueryListAsync<T>(Expression<Func<T, bool>> expression) where T : class, new()
    {
        var info = Provider?.GetSelectCommand(expression);
        return QueryListAsync<T>(info);
    }

    /// <summary>
    /// 异步查询多条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <typeparam name="TKey">ID字段类型。</typeparam>
    /// <param name="ids">实体ID集合。</param>
    /// <returns>多条数据。</returns>
    public virtual Task<List<T>> QueryListByIdAsync<T, TKey>(TKey[] ids) where T : new()
    {
        if (ids == null || ids.Length == 0)
            return Task.FromResult<List<T>>(null);

        var idTexts = new List<string>();
        var paramters = new Dictionary<string, object>();
        for (int i = 0; i < ids.Length; i++)
        {
            idTexts.Add($"{FormatName(nameof(EntityBase.Id))}=@id{i}");
            paramters.Add($"id{i}", ids[i]);
        }

        var where = string.Join(" or ", [.. idTexts]);
        var sb = Provider?.Sql.SelectAll().From<T>().WhereSql(where);
        var sql = sb.ToSqlString();
        var info = new CommandInfo(Provider, typeof(T), sql, paramters);
        return QueryListAsync<T>(info);
    }

    /// <summary>
    /// 异步查询多条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="ids">实体ID集合。</param>
    /// <returns>多条数据。</returns>
    public Task<List<T>> QueryListByIdAsync<T>(string[] ids) where T : new() => QueryListByIdAsync<T, string>(ids);

    /// <summary>
    /// 异步查询多条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="ids">实体ID集合。</param>
    /// <returns>多条数据。</returns>
    public Task<List<T>> QueryListByIdAsync<T>(long[] ids) where T: new() => QueryListByIdAsync<T, long>(ids);

    /// <summary>
    /// 异步查询多条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="ids">实体ID集合。</param>
    /// <returns>多条数据。</returns>
    public Task<List<T>> QueryListByIdAsync<T>(int[] ids) where T : new() => QueryListByIdAsync<T, int>(ids);

    /// <summary>
    /// 异步查询DataTable。
    /// </summary>
    /// <param name="sql">查询SQL语句。</param>
    /// <param name="param">查询参数。</param>
    /// <returns>DataTable。</returns>
    public virtual async Task<DataTable> QueryTableAsync(string sql, object param = null)
    {
        var data = new DataTable();
        var info = new CommandInfo(Provider, sql, param);
        using (var reader = await ExecuteReaderAsync(info))
        {
            if (reader != null)
                data.Load(reader);
        }
        return data;
    }

    /// <summary>
    /// 异步支持多个查询操作，开关一次数据库连接。
    /// </summary>
    /// <param name="action">多个查询操作委托。</param>
    /// <returns></returns>
    public async Task QueryActionAsync(Func<Database, Task> action)
    {
        await OpenAsync();
        await action.Invoke(this);
        await CloseAsync();
    }

    internal async Task<T> QueryAsync<T>(CommandInfo info) where T : new()
    {
        T obj = default;
        try
        {
            using (var reader = await ExecuteReaderAsync(info))
            {
                if (reader != null && reader.Read())
                {
                    obj = (T)DbUtils.ConvertTo<T>(reader);
                }
            }
            if (info.IsClose)
                conn?.Close();
        }
        catch (Exception ex)
        {
            HandException(info, ex);
        }
        return obj;
    }

    internal async Task<List<T>> QueryListAsync<T>(CommandInfo info) where T : new()
    {
        var lists = new List<T>();
        try
        {
            using (var reader = await ExecuteReaderAsync(info))
            {
                while (reader != null && reader.Read())
                {
                    var obj = DbUtils.ConvertTo<T>(reader);
                    lists.Add((T)obj);
                }
            }
            if (info.IsClose)
                conn?.Close();
        }
        catch (Exception ex)
        {
            HandException(info, ex);
        }
        return lists;
    }

    private async Task<IDataReader> ExecuteReaderAsync(CommandInfo info)
    {
        try
        {
            using var cmd = await PrepareCommandAsync(info);
            var reader = info.IsClose
                       ? cmd.ExecuteReader(CommandBehavior.CloseConnection)
                       : cmd.ExecuteReader();
            cmd.Parameters.Clear();
            return reader;
        }
        catch (Exception ex)
        {
            HandException(info, ex);
            return null;
        }
    }
}