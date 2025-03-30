namespace Known.Data;

public partial class Database
{
    /// <summary>
    /// 异步查询标量值。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="sql">查询SQL语句。</param>
    /// <param name="param">查询参数。</param>
    /// <returns>标量值。</returns>
    public virtual Task<T> ScalarAsync<T>(string sql, object param = null)
    {
        var info = new CommandInfo(Provider, typeof(T), sql, param);
        return ScalarAsync<T>(info);
    }

    /// <summary>
    /// 异步查询标量值列表。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="sql">查询SQL语句。</param>
    /// <param name="param">查询参数。</param>
    /// <returns>标量值列表。</returns>
    public virtual Task<List<T>> ScalarsAsync<T>(string sql, object param = null)
    {
        var info = new CommandInfo(Provider, typeof(T), sql, param);
        return ScalarsAsync<T>(info);
    }

    /// <summary>
    /// 异步查询表数据量。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <returns>表数据量。</returns>
    public virtual Task<int> CountAsync<T>() where T : class, new()
    {
        var info = Provider.GetCountCommand<T>();
        return ScalarAsync<int>(info);
    }

    /// <summary>
    /// 异步查询表数据量。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="expression">查询表达式。</param>
    /// <returns>表数据量。</returns>
    public virtual Task<int> CountAsync<T>(Expression<Func<T, bool>> expression) where T : class, new()
    {
        var info = Provider.GetCountCommand(expression);
        return ScalarAsync<int>(info);
    }

    /// <summary>
    /// 异步判断是否存在数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="expression">查询表达式。</param>
    /// <returns>是否存在。</returns>
    public virtual async Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> expression) where T : class, new()
    {
        var count = await CountAsync(expression);
        return count > 0;
    }

    internal async Task<T> ScalarAsync<T>(CommandInfo info)
    {
        try
        {
            var cmd = await PrepareCommandAsync(info);
            var scalar = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            if (info.IsClose)
                conn?.Close();
            return Utils.ConvertTo<T>(scalar);
        }
        catch (Exception ex)
        {
            HandException(info, ex);
            return default;
        }
    }

    private async Task<List<T>> ScalarsAsync<T>(CommandInfo info)
    {
        var data = new List<T>();
        try
        {
            var cmd = await PrepareCommandAsync(info);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var obj = Utils.ConvertTo<T>(reader[0]);
                    data.Add(obj);
                }
            }

            cmd.Parameters.Clear();
            if (info.IsClose)
                conn?.Close();
        }
        catch (Exception ex)
        {
            HandException(info, ex);
        }
        return data;
    }

    /// <summary>
    /// 异步获取实体最大整型ID值。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <returns></returns>
    public Task<int> GetMaxIdAsync<T>()
    {
        var tableName = Provider.GetTableName(typeof(T));
        var sql = $"select max(Id) from {Provider.FormatName(tableName)}";
        return ScalarAsync<int>(sql);
    }
}