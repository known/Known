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
    private const string KeyLog = "Logs";

    public Task<Result> AddLogAsync(LogInfo info)
    {
        info.CreateBy = CurrentUser.UserName;
        info.CreateTime = DateTime.Now;
        return SaveModelAsync(KeyLog, info);
    }

    public Task<PagingResult<LogInfo>> QueryLogsAsync(PagingCriteria criteria)
    {
        return QueryModelsAsync<LogInfo>(KeyLog, criteria);
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