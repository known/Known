namespace Known.Data;

public partial class Database
{
    /// <summary>
    /// 异步存储过程查询DataTable。
    /// </summary>
    /// <param name="spName">存储过程名称。</param>
    /// <param name="parameters">存储过程参数。</param>
    /// <returns>DataTable。</returns>
    public virtual async Task<DataTable> QueryTableSPAsync(string spName, List<DbParamInfo> parameters = null)
    {
        var data = new DataTable();
        var info = new CommandInfo(Provider, spName);
        info.CmdType = CommandType.StoredProcedure;
        info.Parameters = parameters;
        using (var reader = await ExecuteReaderAsync(info))
        {
            if (reader != null)
                data.Load(reader);
        }
        return data;
    }

    /// <summary>
    /// 异步存储过程查询多条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="spName">存储过程名称。</param>
    /// <param name="parameters">存储过程参数。</param>
    /// <returns>多条数据。</returns>
    public virtual Task<List<T>> QueryListSPAsync<T>(string spName, List<DbParamInfo> parameters = null) where T : new()
    {
        var info = new CommandInfo(Provider, typeof(T), spName);
        info.CmdType = CommandType.StoredProcedure;
        info.Parameters = parameters;
        return QueryListAsync<T>(info);
    }

    /// <summary>
    /// 异步存储过程查询单条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="spName">存储过程名称。</param>
    /// <param name="parameters">存储过程参数。</param>
    /// <returns>单条数据。</returns>
    public virtual Task<T> QuerySPAsync<T>(string spName, List<DbParamInfo> parameters = null) where T : new()
    {
        var info = new CommandInfo(Provider, typeof(T), spName);
        info.CmdType = CommandType.StoredProcedure;
        info.Parameters = parameters;
        return QueryAsync<T>(info);
    }

    /// <summary>
    /// 异步存储过程查询标量值。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="spName">存储过程名称。</param>
    /// <param name="parameters">存储过程参数。</param>
    /// <returns>标量值。</returns>
    public virtual Task<T> ScalarSPAsync<T>(string spName, List<DbParamInfo> parameters = null)
    {
        var info = new CommandInfo(Provider, typeof(T), spName);
        info.CmdType = CommandType.StoredProcedure;
        info.Parameters = parameters;
        return ScalarAsync<T>(info);
    }

    /// <summary>
    /// 异步执行存储过程。
    /// </summary>
    /// <param name="spName">存储过程名称。</param>
    /// <param name="parameters">存储过程参数。</param>
    /// <returns>影响的行数。</returns>
    public virtual Task<int> ExecuteSPAsync(string spName, List<DbParamInfo> parameters = null)
    {
        var info = new CommandInfo(Provider, spName);
        info.CmdType = CommandType.StoredProcedure;
        info.Parameters = parameters;
        return ExecuteNonQueryAsync(info);
    }
}