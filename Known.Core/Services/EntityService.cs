namespace Known.Services;

[WebApi]
class EntityService<TEntity>(Context context) : ServiceBase(context), IEntityService<TEntity> where TEntity : EntityBase, new()
{
    [Route("/api/Entities")]
    public Task<PagingResult<TEntity>> QueryAsync(PagingCriteria criteria)
    {
        return Database.QueryPageAsync<TEntity>(criteria);
    }

    [Route("/api/Entity")]
    public Task<TEntity> GetAsync(object id)
    {
        return Database.QueryByIdAsync<TEntity>(id?.ToString());
    }

    [Route("/api/Entity/DeleteEntities")]
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

    [Route("/api/Entity/SaveEntity")]
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