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
    Task<TEntity> GetAsync(object id);

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

class EntityService<TEntity>(Context context) : ServiceBase(context), IEntityService<TEntity>
{
    public Task<PagingResult<TEntity>> QueryAsync(PagingCriteria criteria)
    {
        return Task.FromResult(new PagingResult<TEntity>());
    }

    public Task<TEntity> GetAsync(object id)
    {
        return Task.FromResult(default(TEntity));
    }

    public Task<Result> DeleteAsync(List<TEntity> models)
    {
        return Result.SuccessAsync("添加成功！");
    }

    public Task<Result> SaveAsync(TEntity model)
    {
        return Result.SuccessAsync("保存成功！");
    }
}

class EntityClient<TEntity>(HttpClient http) : ClientBase(http), IEntityService<TEntity>
{
    public Task<PagingResult<TEntity>> QueryAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<TEntity>("/Entity/QueryEntities", criteria);
    }

    public Task<TEntity> GetAsync(object id)
    {
        return Http.GetAsync<TEntity>($"/Entity?id={id}");
    }

    public Task<Result> DeleteAsync(List<TEntity> models)
    {
        return Http.PostAsync("/Entity/DeleteEntities", models);
    }

    public Task<Result> SaveAsync(TEntity model)
    {
        return Http.PostAsync("/Entity/SaveEntity", model);
    }
}