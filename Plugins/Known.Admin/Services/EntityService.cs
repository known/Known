namespace Known.Services;

class EntityService<TEntity>(Context context) : ServiceBase(context), IEntityService<TEntity> where TEntity : EntityBase, new()
{
    public Task<PagingResult<TEntity>> QueryAsync(PagingCriteria criteria)
    {
        return Database.QueryPageAsync<TEntity>(criteria);
    }

    public Task<TEntity> GetAsync(object id)
    {
        return Database.QueryByIdAsync<TEntity>(id?.ToString());
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