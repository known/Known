namespace Known.Data;

public partial class Database
{
    /// <summary>
    /// 异步插入多条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <typeparam name="TKey">主键属性类型。</typeparam>
    /// <param name="entities">对象列表。</param>
    /// <returns></returns>
    public virtual async Task InsertAsync<T, TKey>(List<T> entities) where T : EntityBase<TKey>, new()
    {
        if (entities == null || entities.Count == 0)
            return;

        var maxId = await GetMaxIdAsync<T>();
        foreach (var entity in entities)
        {
            entity.IsNew = true;
            if (entity.Id.Equals("-1"))
                entity.Id = Utils.ConvertTo<TKey>(++maxId);
            await SaveAsync<T, TKey>(entity, false);
        }
    }

    /// <summary>
    /// 异步插入实体对象。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <typeparam name="TKey">主键属性类型。</typeparam>
    /// <param name="entity">实体对象。</param>
    /// <returns>实体对象。</returns>
    public virtual async Task<T> InsertAsync<T, TKey>(T entity) where T : EntityBase<TKey>, new()
    {
        if (entity == null)
            return entity;

        entity.IsNew = true;
        await SaveAsync<T, TKey>(entity, false);
        return entity;
    }

    /// <summary>
    /// 异步删除实体数据。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <typeparam name="TKey">主键属性类型。</typeparam>
    /// <param name="entity">实体对象。</param>
    /// <returns>影响的行数。</returns>
    public virtual Task<int> DeleteAsync<T, TKey>(T entity) where T : EntityBase<TKey>, new()
    {
        if (entity == null)
            return Task.FromResult(0);

        return DeleteAsync<T, TKey>(entity.Id);
    }

    /// <summary>
    /// 异步删除实体数据。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <typeparam name="TKey">主键属性类型。</typeparam>
    /// <param name="id">ID字段值。</param>
    /// <returns>影响的行数。</returns>
    public Task<int> DeleteAsync<T, TKey>(TKey id) where T : EntityBase<TKey>, new()
    {
        if (id == null)
            return Task.FromResult(0);

        return DeleteAsync<T>(d => d.Id.Equals(id));
    }

    /// <summary>
    /// 异步保存实体对象。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <typeparam name="TKey">主键属性类型。</typeparam>
    /// <param name="entity">实体对象。</param>
    /// <returns></returns>
    public virtual Task SaveAsync<T, TKey>(T entity) where T : EntityBase<TKey>, new()
    {
        return SaveAsync<T, TKey>(entity, true);
    }

    /// <summary>
    /// 异步保存实体对象。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <typeparam name="TKey">主键属性类型。</typeparam>
    /// <param name="entity">实体对象。</param>
    /// <returns>影响的行数。</returns>
    protected virtual Task<int> SaveDataAsync<T, TKey>(T entity) where T : EntityBase<TKey>, new()
    {
        var info = entity.IsNew
                 ? Provider.GetInsertCommand(entity)
                 : Provider.GetUpdateCommand<T, TKey>(entity);
        info.IsSave = true;
        info.Original = entity.Original;
        return ExecuteNonQueryAsync(info);
    }

    /// <summary>
    /// 检查实体对象状态（新增/修改）。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <typeparam name="TKey">主键属性类型。</typeparam>
    /// <param name="entity">实体对象。</param>
    protected virtual void CheckEntity<T, TKey>(T entity) where T : EntityBase<TKey>, new() { }

    private async Task SaveAsync<T, TKey>(T entity, bool isCheckEntity) where T : EntityBase<TKey>, new()
    {
        if (entity == null)
            return;

        if (isCheckEntity)
            CheckEntity<T, TKey>(entity);

        if (!entity.IsNew)
            await SetOriginalAsync<T, TKey>(entity);

        if (entity.IsNew && entity.Id.Equals("-1"))
        {
            var maxId = await GetMaxIdAsync<T>();
            entity.Id = Utils.ConvertTo<TKey>(++maxId);
        }

        await SaveDataAsync<T, TKey>(entity);
        entity.IsNew = false;
    }

    private async Task SetOriginalAsync<T, TKey>(T entity) where T : EntityBase<TKey>, new()
    {
        DbMonitor.OnSql(new CommandInfo { Text = "The follow SQL is a update query" });
        var info = Provider?.GetSelectCommand<T>(d => d.Id.Equals(entity.Id));
        var original = await QueryAsync<Dictionary<string, object>>(info);
        entity.SetOriginal(original);
    }
}