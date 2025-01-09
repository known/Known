namespace Known.Services;

public partial interface IAdminService
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

partial class AdminService
{
    public Task<PagingResult<SysTask>> QueryTasksAsync(PagingCriteria criteria)
    {
        return Task.FromResult(new PagingResult<SysTask>());
    }

    public Task<Result> DeleteTasksAsync(List<SysTask> models)
    {
        return Result.SuccessAsync("删除成功！");
    }

    public Task<Result> ResetTasksAsync(List<SysTask> models)
    {
        return Result.SuccessAsync("保存成功！");
    }
}

partial class AdminClient
{
    public Task<PagingResult<SysTask>> QueryTasksAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<SysTask>("/Admin/QueryTasks", criteria);
    }

    public Task<Result> DeleteTasksAsync(List<SysTask> models)
    {
        return Http.PostAsync("/Admin/DeleteTasks", models);
    }

    public Task<Result> ResetTasksAsync(List<SysTask> models)
    {
        return Http.PostAsync("/Admin/ResetTasks", models);
    }
}