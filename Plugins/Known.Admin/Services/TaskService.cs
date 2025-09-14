namespace Known.Services;

public interface ITaskService : IService
{
    Task<PagingResult<TaskInfo>> QueryTasksAsync(PagingCriteria criteria);
    Task<Result> DeleteTasksAsync(List<TaskInfo> infos);
    Task<Result> ResetTasksAsync(List<TaskInfo> infos);
}

[Client]
class TaskClient(HttpClient http) : ClientBase(http), ITaskService
{
    public Task<PagingResult<TaskInfo>> QueryTasksAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<TaskInfo>("/Task/QueryTasks", criteria);
    }

    public Task<Result> DeleteTasksAsync(List<TaskInfo> infos)
    {
        return Http.PostAsync("/Task/DeleteTasks", infos);
    }

    public Task<Result> ResetTasksAsync(List<TaskInfo> infos)
    {
        return Http.PostAsync("/Task/ResetTasks", infos);
    }
}

[WebApi, Service]
class TaskService(Context context) : ServiceBase(context), ITaskService
{
    public Task<PagingResult<TaskInfo>> QueryTasksAsync(PagingCriteria criteria)
    {
        if (criteria.OrderBys == null || criteria.OrderBys.Length == 0)
            criteria.OrderBys = [$"{nameof(TaskInfo.CreateTime)} desc"];
        return Database.Query<SysTask>(criteria).ToPageAsync<TaskInfo>();
    }

    public async Task<Result> DeleteTasksAsync(List<TaskInfo> infos)
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

    public async Task<Result> ResetTasksAsync(List<TaskInfo> infos)
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