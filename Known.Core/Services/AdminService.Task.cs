namespace Known.Services;

partial class AdminService
{
    public Task<PagingResult<SysTask>> QueryTasksAsync(PagingCriteria criteria)
    {
        if (criteria.OrderBys == null || criteria.OrderBys.Length == 0)
            criteria.OrderBys = [$"{nameof(SysTask.CreateTime)} desc"];
        return Database.QueryPageAsync<SysTask>(criteria);
    }

    public async Task<Result> DeleteTasksAsync(List<SysTask> models)
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

    public async Task<Result> ResetTasksAsync(List<SysTask> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Reset, async db =>
        {
            foreach (var item in models)
            {
                item.Status = TaskJobStatus.Pending;
                await db.SaveAsync(item);
            }
        });
    }
}