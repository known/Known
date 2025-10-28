namespace Known.Services;

/// <summary>
/// 后台任务服务接口。
/// </summary>
public interface ITaskService : IService
{
    /// <summary>
    /// 异步分页查询后台任务。
    /// </summary>
    /// <param name="criteria">查询条件。</param>
    /// <returns></returns>
    Task<PagingResult<SysTask>> QueryTasksAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步删除后台任务。
    /// </summary>
    /// <param name="infos">任务列表。</param>
    /// <returns></returns>
    Task<Result> DeleteTasksAsync(List<SysTask> infos);

    /// <summary>
    /// 异步重置后台任务。
    /// </summary>
    /// <param name="infos">任务列表。</param>
    /// <returns></returns>
    Task<Result> ResetTasksAsync(List<SysTask> infos);
}

[Client]
class TaskClient(HttpClient http) : ClientBase(http), ITaskService
{
    public Task<PagingResult<SysTask>> QueryTasksAsync(PagingCriteria criteria) => Http.QueryAsync<SysTask>("/Task/QueryTasks", criteria);
    public Task<Result> DeleteTasksAsync(List<SysTask> infos) => Http.PostAsync("/Task/DeleteTasks", infos);
    public Task<Result> ResetTasksAsync(List<SysTask> infos) => Http.PostAsync("/Task/ResetTasks", infos);
}

[WebApi, Service]
class TaskService(Context context) : ServiceBase(context), ITaskService
{
    public Task<PagingResult<SysTask>> QueryTasksAsync(PagingCriteria criteria)
    {
        if (criteria.OrderBys == null || criteria.OrderBys.Length == 0)
            criteria.OrderBys = [$"{nameof(SysTask.CreateTime)} desc"];
        return Database.QueryPageAsync<SysTask>(criteria);
    }

    public async Task<Result> DeleteTasksAsync(List<SysTask> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in infos)
            {
                await db.DeleteAsync<SysTask>(item.Id);
            }
        });
    }

    public async Task<Result> ResetTasksAsync(List<SysTask> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        await Database.QueryActionAsync(async db =>
        {
            foreach (var item in infos)
            {
                var task = await db.QueryByIdAsync<SysTask>(item.Id);
                if (task != null)
                {
                    task.Status = TaskJobStatus.Pending;
                    await db.SaveAsync(task);
                }
                item.File = await db.Query<SysFile>().FirstAsync<AttachInfo>(d => d.Id == item.Target);
                TaskHelper.NotifyRun(item, Context);
            }
        });
        return Result.Success(Language.Success(Language.Reset));
    }
}