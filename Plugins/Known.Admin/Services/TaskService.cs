namespace Known.Services;

/// <summary>
/// 后台任务服务接口。
/// </summary>
public interface ITaskService : IService
{
    /// <summary>
    /// 异步分页查询系统后台任务。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<SysTask>> QueryTasksAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步删除系统后台任务。
    /// </summary>
    /// <param name="models">系统后台任务列表。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteTasksAsync(List<SysTask> models);

    /// <summary>
    /// 异步重置系统后台任务。
    /// </summary>
    /// <param name="models">系统后台任务列表。</param>
    /// <returns>重置结果。</returns>
    Task<Result> ResetTasksAsync(List<SysTask> models);
}

class TaskService(Context context) : ServiceBase(context), ITaskService
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