namespace Known.Data;

public partial class Database
{
    /// <summary>
    /// 异步查询分页数据。
    /// </summary>
    /// <param name="tableName">数据库表名。</param>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页查询结果。</returns>
    public virtual Task<PagingResult<Dictionary<string, object>>> QueryPageAsync(string tableName, PagingCriteria criteria)
    {
        var sql = Provider.GetSelectSql(tableName);
        return QueryPageAsync<Dictionary<string, object>>(sql, criteria);
    }

    /// <summary>
    /// 异步判断表中是否存在ID。
    /// </summary>
    /// <param name="tableName">数据库表名。</param>
    /// <param name="id">ID字段值。</param>
    /// <returns>是否存在。</returns>
    public virtual async Task<bool> ExistsAsync(string tableName, string id)
    {
        var info = Provider.GetCountCommand(tableName, id);
        var count = await ScalarAsync<int>(info);
        return count > 0;
    }

    /// <summary>
    /// 异步删除数据。
    /// </summary>
    /// <param name="tableName">数据库表名。</param>
    /// <param name="id">ID字段值。</param>
    /// <returns>影响的行数。</returns>
    public virtual Task<int> DeleteAsync(string tableName, string id)
    {
        if (string.IsNullOrEmpty(id))
            return Task.FromResult(0);

        var info = Provider.GetDeleteCommand(tableName, id);
        return ExecuteNonQueryAsync(info);
    }

    /// <summary>
    /// 异步插入字典对象。
    /// </summary>
    /// <param name="tableName">数据库表名。</param>
    /// <param name="data">字典对象。</param>
    /// <returns>影响的行数。</returns>
    public virtual Task<int> InsertAsync(string tableName, Dictionary<string, object> data)
    {
        if (data == null || data.Count == 0)
            return Task.FromResult(0);

        var info = Provider.GetInsertCommand(tableName, data);
        return ExecuteNonQueryAsync(info);
    }

    /// <summary>
    /// 异步修改字典对象。
    /// </summary>
    /// <param name="tableName">数据库表名。</param>
    /// <param name="keyField">主键字段字符串，多个字段用逗号分割。</param>
    /// <param name="data">字典对象。</param>
    /// <returns>影响的行数。</returns>
    public virtual Task<int> UpdateAsync(string tableName, string keyField, Dictionary<string, object> data)
    {
        if (data == null || data.Count == 0)
            return Task.FromResult(0);

        var info = Provider.GetUpdateCommand(tableName, keyField, data);
        return ExecuteNonQueryAsync(info);
    }

    /// <summary>
    /// 异步保存字典对象。
    /// </summary>
    /// <param name="tableName">数据库表名。</param>
    /// <param name="data">字典对象。</param>
    /// <returns>影响的行数。</returns>
    public virtual async Task<int> SaveAsync(string tableName, Dictionary<string, object> data)
    {
        if (data == null || data.Count == 0)
            return 0;

        var id = data.GetValue<string>(nameof(EntityBase.Id));
        if (await ExistsAsync(tableName, id))
        {
            var version = data.GetValue<int>(nameof(EntityBase.Version)) + 1;
            data.SetValue(nameof(EntityBase.Version), version);
            return await UpdateAsync(tableName, nameof(EntityBase.Id), data);
        }

        if (string.IsNullOrWhiteSpace(id))
            data.SetValue(nameof(EntityBase.Id), Utils.GetGuid());
        data.SetValue(nameof(EntityBase.CreateBy), User.UserName);
        data.SetValue(nameof(EntityBase.CreateTime), DateTime.Now);
        data.SetValue(nameof(EntityBase.ModifyBy), User.UserName);
        data.SetValue(nameof(EntityBase.ModifyTime), DateTime.Now);
        data.SetValue(nameof(EntityBase.Version), 1);
        data.SetValue(nameof(EntityBase.AppId), User.AppId);
        data.SetValue(nameof(EntityBase.CompNo), User.CompNo);
        return await InsertAsync(tableName, data);
    }
}