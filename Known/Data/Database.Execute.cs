namespace Known.Data;

public partial class Database
{
    /// <summary>
    /// 异步执行SQL语句。
    /// </summary>
    /// <param name="sql">SQL语句。</param>
    /// <param name="param">查询参数。</param>
    /// <returns>影响的行数。</returns>
    public virtual Task<int> ExecuteAsync(string sql, object param = null)
    {
        var info = Provider.GetCommand(sql, param);
        return ExecuteNonQueryAsync(info);
    }

    /// <summary>
    /// 异步删除数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="expression">查询表达式。</param>
    /// <returns>影响的行数。</returns>
    public virtual Task<int> DeleteAsync<T>(Expression<Func<T, bool>> expression) where T : class, new()
    {
        var info = Provider.GetDeleteCommand(expression);
        return ExecuteNonQueryAsync(info);
    }

    /// <summary>
    /// 异步删除全部数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <returns>影响的行数。</returns>
    public virtual Task<int> DeleteAllAsync<T>() where T : class, new()
    {
        var info = Provider.GetDeleteCommand<T>();
        return ExecuteNonQueryAsync(info);
    }

    /// <summary>
    /// 异步插入单条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="data">数据对象。</param>
    /// <returns>影响的行数。</returns>
    public virtual Task<int> InsertAsync<T>(T data) where T : class, new()
    {
        if (data == null)
            return Task.FromResult(0);

        var info = Provider.GetInsertCommand(data);
        return ExecuteNonQueryAsync(info);
    }

    /// <summary>
    /// 异步插入多条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="datas">数据对象列表。</param>
    /// <returns>影响的行数。</returns>
    public virtual async Task<int> InsertListAsync<T>(List<T> datas) where T : class, new()
    {
        if (datas == null || datas.Count == 0)
            return 0;

        var close = false;
        if (conn != null && conn.State != ConnectionState.Open)
        {
            close = true;
            conn.Open();
        }

        var count = 0;
        var info = Provider.GetInsertCommand<T>();
        foreach (var item in datas)
        {
            info.SetParameters(item);
            count += await ExecuteNonQueryAsync(info);
        }

        if (conn != null && close)
            conn.Close();

        return count;
    }

    /// <summary>
    /// 异步插入DataTable。
    /// </summary>
    /// <param name="data">DataTable对象。</param>
    /// <returns>影响的行数。</returns>
    public virtual async Task<int> InsertTableAsync(DataTable data)
    {
        if (data == null || data.Rows.Count == 0)
            return 0;

        var count = 0;
        var info = Provider.GetInsertCommand(data);
        foreach (DataRow item in data.Rows)
        {
            info.SetParameters(item);
            count += await ExecuteNonQueryAsync(info);
        }
        return count;
    }

    /// <summary>
    /// 异步保存实体对象。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <param name="entity">实体对象。</param>
    /// <returns></returns>
    public virtual Task SaveAsync<T>(T entity) where T : EntityBase, new()
    {
        return SaveAsync(entity, true);
    }

    /// <summary>
    /// 异步保存多条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="entities">对象列表。</param>
    /// <returns></returns>
    public virtual async Task InsertAsync<T>(List<T> entities) where T : EntityBase, new()
    {
        if (entities == null || entities.Count == 0)
            return;

        foreach (var entity in entities)
        {
            entity.IsNew = true;
            await SaveAsync(entity, false);
        }
    }

    /// <summary>
    /// 异步插入实体对象。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <param name="entity">实体对象。</param>
    /// <param name="newId">是否重新生成新ID。</param>
    /// <returns>实体对象。</returns>
    public virtual async Task<T> InsertAsync<T>(T entity, bool newId) where T : EntityBase, new()
    {
        if (entity == null)
            return entity;

        if (newId)
            entity.Id = Utils.GetGuid();
        entity.IsNew = true;
        await SaveAsync(entity, false);
        return entity;
    }

    /// <summary>
    /// 异步删除实体数据。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <param name="entity">实体对象。</param>
    /// <returns>影响的行数。</returns>
    public virtual Task<int> DeleteAsync<T>(T entity) where T : EntityBase, new()
    {
        return DeleteAsync<T>(entity?.Id);
    }

    /// <summary>
    /// 异步删除实体数据。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <param name="id">ID字段值。</param>
    /// <returns>影响的行数。</returns>
    public Task<int> DeleteAsync<T>(string id) where T : EntityBase, new()
    {
        if (string.IsNullOrWhiteSpace(id))
            return Task.FromResult(0);

        return DeleteAsync<T>(d => d.Id == id);
    }

    /// <summary>
    /// 异步保存实体对象。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <param name="entity">实体对象。</param>
    /// <returns>影响的行数。</returns>
    protected virtual Task<int> SaveDataAsync<T>(T entity) where T : EntityBase, new()
    {
        var info = Provider.GetSaveCommand(entity);
        info.IsSave = true;
        return ExecuteNonQueryAsync(info);
    }

    private async Task SaveAsync<T>(T entity, bool isCheckEntity) where T : EntityBase, new()
    {
        if (entity == null)
            return;

        if (isCheckEntity)
            CheckEntity(entity);

        var none = "Anonymous";
        if (entity.IsNew)
        {
            if (entity.CreateBy == "temp")
                entity.CreateBy = User?.UserName ?? none;
            entity.CreateTime = DateTime.Now;
            entity.Version = 1;
            if (entity.AppId == "temp")
                entity.AppId = User?.AppId ?? Config.App.Id;
            if (entity.CompNo == "temp")
                entity.CompNo = User?.CompNo ?? none;
        }
        else
        {
            entity.Version += 1;
            await SetOriginalAsync(entity);
        }

        entity.ModifyBy = User?.UserName ?? none;
        entity.ModifyTime = DateTime.Now;
        await SaveDataAsync(entity);
        entity.IsNew = false;
    }

    private async Task SetOriginalAsync<T>(T entity) where T : EntityBase, new()
    {
        var info = Provider.GetSelectCommand<T>(entity.Id);
        var original = await QueryAsync<Dictionary<string, object>>(info);
        entity.SetOriginal(original);
    }

    private async Task<int> ExecuteNonQueryAsync(CommandInfo info)
    {
        try
        {
            var cmd = await PrepareCommandAsync(info);
            var value = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            if (info.IsClose)
                conn?.Close();
            return value;
        }
        catch (Exception ex)
        {
            HandException(info, ex);
            throw new SystemException(ex.Message, ex);
        }
    }
}