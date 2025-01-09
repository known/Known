namespace Known.Services;

public partial interface IAdminService
{
    /// <summary>
    /// 异步添加系统日志。
    /// </summary>
    /// <param name="log">系统日志</param>
    /// <returns>添加结果。</returns>
    Task<Result> AddLogAsync(LogInfo log);

    /// <summary>
    /// 异步分页查询系统日志。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<SysLog>> QueryLogsAsync(PagingCriteria criteria);
}

partial class AdminService
{
    public Task<Result> AddLogAsync(LogInfo log)
    {
        return Result.SuccessAsync("添加成功！");
    }

    public Task<PagingResult<SysLog>> QueryLogsAsync(PagingCriteria criteria)
    {
        return Task.FromResult(new PagingResult<SysLog>());
    }
}

partial class AdminClient
{
    public Task<Result> AddLogAsync(LogInfo log)
    {
        return Http.PostAsync("/Admin/AddLog", log);
    }

    public Task<PagingResult<SysLog>> QueryLogsAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<SysLog>("/Admin/QueryLogs", criteria);
    }
}