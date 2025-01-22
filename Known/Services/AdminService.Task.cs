namespace Known.Services;

public partial interface IAdminService
{
    /// <summary>
    /// 异步分页查询系统后台任务。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<TaskInfo>> QueryTasksAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步删除系统后台任务。
    /// </summary>
    /// <param name="infos">系统后台任务列表。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteTasksAsync(List<TaskInfo> infos);

    /// <summary>
    /// 异步重置系统后台任务。
    /// </summary>
    /// <param name="infos">系统后台任务列表。</param>
    /// <returns>重置结果。</returns>
    Task<Result> ResetTasksAsync(List<TaskInfo> infos);
}

partial class AdminService
{
    public Task<PagingResult<TaskInfo>> QueryTasksAsync(PagingCriteria criteria)
    {
        return Task.FromResult(new PagingResult<TaskInfo>());
    }

    public Task<Result> DeleteTasksAsync(List<TaskInfo> infos)
    {
        return Result.SuccessAsync(Language.Success(Language.Delete));
    }

    public Task<Result> ResetTasksAsync(List<TaskInfo> infos)
    {
        return Result.SuccessAsync(Language.Success(Language.Save));
    }
}

partial class AdminClient
{
    public Task<PagingResult<TaskInfo>> QueryTasksAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<TaskInfo>("/Admin/QueryTasks", criteria);
    }

    public Task<Result> DeleteTasksAsync(List<TaskInfo> infos)
    {
        return Http.PostAsync("/Admin/DeleteTasks", infos);
    }

    public Task<Result> ResetTasksAsync(List<TaskInfo> infos)
    {
        return Http.PostAsync("/Admin/ResetTasks", infos);
    }
}