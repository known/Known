﻿namespace Known.Data;

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
        var sql = $"select * from {FormatName(tableName)}";
        return QueryPageAsync<Dictionary<string, object>>(sql, criteria);
    }

    /// <summary>
    /// 异步查询数据。
    /// </summary>
    /// <param name="tableName">数据库表名。</param>
    /// <param name="id">ID。</param>
    /// <param name="idField">ID字段名。</param>
    /// <returns></returns>
    public virtual Task<Dictionary<string, object>> QueryByIdAsync(string tableName, string id, string idField = nameof(EntityBase.Id))
    {
        var sql = $"select * from {FormatName(tableName)} where {FormatName(idField)}=@id";
        var info = new CommandInfo(Provider, null, tableName, sql, new { id });
        return QueryAsync<Dictionary<string, object>>(sql, info);
    }

    /// <summary>
    /// 异步判断表中是否存在ID。
    /// </summary>
    /// <param name="tableName">数据库表名。</param>
    /// <param name="id">ID字段值。</param>
    /// <returns>是否存在。</returns>
    public virtual Task<bool> ExistsAsync(string tableName, string id)
    {
        return ExistsAsync(tableName, id, nameof(EntityBase.Id));
    }

    /// <summary>
    /// 异步判断表中是否存在ID。
    /// </summary>
    /// <param name="tableName">数据库表名。</param>
    /// <param name="id">ID字段值。</param>
    /// <param name="idField">ID字段名。</param>
    /// <returns>是否存在。</returns>
    public virtual async Task<bool> ExistsAsync(string tableName, string id, string idField)
    {
        var sql = $"select count(*) from {FormatName(tableName)} where {FormatName(idField)}=@id";
        var info = new CommandInfo(Provider, null, tableName, sql, new { id });
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
        return DeleteAsync(tableName, id, nameof(EntityBase.Id));
    }

    /// <summary>
    /// 异步删除数据。
    /// </summary>
    /// <param name="tableName">数据库表名。</param>
    /// <param name="id">ID字段值。</param>
    /// <param name="idField">ID字段名。</param>
    /// <returns>影响的行数。</returns>
    public virtual async Task<int> DeleteAsync(string tableName, string id, string idField)
    {
        if (string.IsNullOrEmpty(id))
            return 0;

        var sql = $"delete from {FormatName(tableName)} where {FormatName(idField)}=@id";
        var info = new CommandInfo(Provider, null, tableName, sql, new { id });
        if (DatabaseOption.Instance.HasOperateMonitor)
        {
            var sql1 = $"select * from {FormatName(tableName)} where {FormatName(idField)}=@id";
            info.DeleteItems = await QueryListAsync<Dictionary<string, object>>(sql, new { id });
        }
        return await ExecuteNonQueryAsync(info);
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

        var changes = new Dictionary<string, object>();
        foreach (var item in data)
        {
            if (item.Value != null)
                changes[item.Key] = item.Value;
        }

        var keys = new List<string>();
        foreach (var key in changes.Keys)
        {
            keys.Add(key);
        }
        var cloumn = string.Join(",", keys.Select(FormatName).ToArray());
        var value = string.Join(",", keys.Select(k => $"@{k}").ToArray());
        var sql = $"insert into {FormatName(tableName)}({cloumn}) values({value})";
        var info = new CommandInfo(Provider, null, tableName, sql, changes);
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

        var changeKeys = new List<string>();
        foreach (var key in data.Keys)
        {
            changeKeys.Add($"{FormatName(key)}=@{key}");
        }
        var column = string.Join(",", [.. changeKeys]);

        var keyFields = new List<string>();
        var keys = keyField.Split(',');
        foreach (var key in keys)
        {
            keyFields.Add($"{FormatName(key)}=@{key}");
        }
        var where = string.Join(" and ", keyFields);
        var sql = $"update {FormatName(tableName)} set {column} where {where}";
        var info = new CommandInfo(Provider, null, tableName, sql, data);
        return ExecuteNonQueryAsync(info);
    }

    /// <summary>
    /// 异步保存字典对象。
    /// </summary>
    /// <param name="tableName">数据库表名。</param>
    /// <param name="data">字典对象。</param>
    /// <param name="idField">ID字段名。</param>
    /// <param name="isEntity">表是否包含EntityBase字段。</param>
    /// <returns>影响的行数。</returns>
    public virtual async Task<int> SaveAsync(string tableName, Dictionary<string, object> data, string idField = nameof(EntityBase.Id), bool isEntity = true)
    {
        if (data == null || data.Count == 0)
            return 0;

        var id = data.GetValue<string>(idField);
        if (await ExistsAsync(tableName, id, idField))
        {
            var version = data.GetValue<int>(nameof(EntityBase.Version)) + 1;
            if (isEntity)
                data.SetValue(nameof(EntityBase.Version), version);
            return await UpdateAsync(tableName, idField, data);
        }

        if (string.IsNullOrWhiteSpace(id))
            data.SetValue(idField, Utils.GetNextId());
        if (isEntity)
        {
            data.SetValue(nameof(EntityBase.CreateBy), User.UserName);
            data.SetValue(nameof(EntityBase.CreateTime), DateTime.Now);
            data.SetValue(nameof(EntityBase.ModifyBy), User.UserName);
            data.SetValue(nameof(EntityBase.ModifyTime), DateTime.Now);
            data.SetValue(nameof(EntityBase.Version), 1);
            data.SetValue(nameof(EntityBase.AppId), User.AppId);
            data.SetValue(nameof(EntityBase.CompNo), User.CompNo);
        }
        return await InsertAsync(tableName, data);
    }
}