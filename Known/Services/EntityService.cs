namespace Known.Services;

/// <summary>
/// 泛型实体自动页面服务接口。
/// </summary>
/// <typeparam name="TEntity">实体类型。</typeparam>
public interface IEntityService<TEntity> : IService
{
    /// <summary>
    /// 异步分页查询实体列表。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<TEntity>> QueryAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步获取指定ID的实体对象。
    /// </summary>
    /// <param name="id">实体对象ID。</param>
    /// <returns>实体对象。</returns>
    Task<TEntity> GetAsync(string id);

    /// <summary>
    /// 异步批量删除实体对象。
    /// </summary>
    /// <param name="models">实体对象列表。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteAsync(List<TEntity> models);

    /// <summary>
    /// 异步保存实体对象。
    /// </summary>
    /// <param name="model">实体对象。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveAsync(TEntity model);
}


class EntityService<TEntity>(Context context) : ServiceBase(context), IEntityService<TEntity> where TEntity : EntityBase, new()
{
    public Task<PagingResult<TEntity>> QueryAsync(PagingCriteria criteria)
    {
        return Database.QueryPageAsync<TEntity>(criteria);
    }

    public Task<TEntity> GetAsync(string id)
    {
        return Database.QueryByIdAsync<TEntity>(id);
    }

    public async Task<Result> DeleteAsync(List<TEntity> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in models)
            {
                await db.DeleteAsync(item);
            }
        });
    }

    public async Task<Result> SaveAsync(TEntity model)
    {
        var vr = model.Validate(Context);
        if (!vr.IsValid)
            return vr;

        return await Database.TransactionAsync(Language.Save, async db =>
        {
            await db.SaveAsync(model);
        }, model);
    }
}