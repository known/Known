namespace Known.Data;

public partial class Database
{
    private List<string> Tables { get; set; }

    /// <summary>
    /// 异步判断是否存在数据表。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <returns>是否存在。</returns>
    public virtual Task<bool> ExistsAsync<T>()
    {
        var tableName = Provider.GetTableName(typeof(T));
        return ExistsTableAsync(tableName);
    }

    /// <summary>
    /// 异步获取数据库所有数据表名。
    /// </summary>
    /// <returns></returns>
    public Task<List<string>> GetTablesAsync()
    {
        var sql = Provider.GetTableSql("");
        if (string.IsNullOrWhiteSpace(sql))
            return Task.FromResult(new List<string>());

        return ScalarsAsync<string>(sql);
    }

    /// <summary>
    /// 异步创建数据库表。
    /// </summary>
    /// <param name="info">数据实体模型。</param>
    /// <returns></returns>
    public async Task CreateTableAsync(DbModelInfo info)
    {
        var tableName = Provider.GetTableName(info.Type);
        if (await ExistsTableAsync(tableName))
            return;

        info.InitFields();
        if (info.Fields == null || info.Fields.Count == 0)
            return;

        var script = Provider.GetTableScript(tableName, info);
        if (string.IsNullOrWhiteSpace(script))
            return;

        await ExecuteAsync(script);
    }

    private async Task<bool> ExistsTableAsync(string tableName)
    {
        Tables ??= await GetTablesAsync();
        return Tables.Exists(t => t.Equals(tableName, StringComparison.OrdinalIgnoreCase));
    }
}