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
    /// 异步分页查询Web日志。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<LogInfo>> QueryWebLogsAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步添加Web日志。
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    Task<Result> AddWebLogAsync(LogInfo info);

    /// <summary>
    /// 异步删除Web日志信息。
    /// </summary>
    /// <param name="infos">信息列表。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteWebLogsAsync(List<LogInfo> infos);

    /// <summary>
    /// 异步清空Web日志信息。
    /// </summary>
    /// <returns>清空结果。</returns>
    Task<Result> ClearWebLogsAsync();
}

partial class AdminClient
{
    public Task<Result> AddLogAsync(LogInfo info) => Http.PostAsync("/Admin/AddLog", info);
    public Task<PagingResult<LogInfo>> QueryWebLogsAsync(PagingCriteria criteria) => Http.QueryAsync<LogInfo>("/Admin/QueryWebLogs", criteria);
    public Task<Result> AddWebLogAsync(LogInfo info) => Http.PostAsync("/Admin/AddWebLog", info);
    public Task<Result> DeleteWebLogsAsync(List<LogInfo> infos) => Http.PostAsync("/Admin/DeleteWebLogs", infos);
    public Task<Result> ClearWebLogsAsync() => Http.PostAsync("/Admin/ClearWebLogs");
}

partial class AdminService
{
    public Task<Result> AddLogAsync(LogInfo info)
    {
        return Database.AddLogAsync(info);
    }

    public Task<PagingResult<LogInfo>> QueryWebLogsAsync(PagingCriteria criteria)
    {
        return Logger.QueryLogsAsync(criteria);
    }

    public Task<Result> AddWebLogAsync(LogInfo info)
    {
        Logger.Logs.Add(info);
        return Result.SuccessAsync(Language.AddSuccess);
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