namespace Known.Services;

public partial interface IAdminService
{
    /// <summary>
    /// 异步添加系统日志。
    /// </summary>
    /// <param name="info">系统日志</param>
    /// <returns>添加结果。</returns>
    Task<Result> AddLogAsync(LogInfo info);

    /// <summary>
    /// 异步分页查询系统日志。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<LogInfo>> QueryLogsAsync(PagingCriteria criteria);
}

partial class AdminService
{
    public Task<Result> AddLogAsync(LogInfo info)
    {
        return Result.SuccessAsync("添加成功！");
    }

    public Task<PagingResult<LogInfo>> QueryLogsAsync(PagingCriteria criteria)
    {
        return Task.FromResult(new PagingResult<LogInfo>());
    }
}

partial class AdminClient
{
    public Task<Result> AddLogAsync(LogInfo info)
    {
        return Http.PostAsync("/Admin/AddLog", info);
    }

    public Task<PagingResult<LogInfo>> QueryLogsAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<LogInfo>("/Admin/QueryLogs", criteria);
    }
}