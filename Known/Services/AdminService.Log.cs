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

    /// <summary>
    /// 异步分页查询Web日志。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<LogInfo>> QueryWebLogsAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步删除日志信息。
    /// </summary>
    /// <param name="infos">信息列表。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteWebLogsAsync(List<LogInfo> infos);

    /// <summary>
    /// 异步清空日志信息。
    /// </summary>
    /// <returns>清空结果。</returns>
    Task<Result> ClearWebLogsAsync();
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

    public Task<PagingResult<LogInfo>> QueryWebLogsAsync(PagingCriteria criteria)
    {
        return Logger.QueryLogsAsync(criteria);
    }

    public Task<Result> DeleteWebLogsAsync(List<LogInfo> infos)
    {
        return Logger.DeleteLogsAsync(infos);
    }

    public Task<Result> ClearWebLogsAsync()
    {
        return Logger.ClearLogsAsync();
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

    public Task<PagingResult<LogInfo>> QueryWebLogsAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<LogInfo>("/Admin/QueryWebLogs", criteria);
    }

    public Task<Result> DeleteWebLogsAsync(List<LogInfo> infos)
    {
        return Http.PostAsync("/Admin/DeleteLogs", infos);
    }

    public Task<Result> ClearWebLogsAsync()
    {
        return Http.PostAsync("/Admin/ClearLogs");
    }
}