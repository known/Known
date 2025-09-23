namespace Known.Services;

public partial interface IPlatformService
{
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

partial class PlatformClient
{
    public Task<PagingResult<LogInfo>> QueryWebLogsAsync(PagingCriteria criteria) => Http.QueryAsync<LogInfo>("/Platform/QueryWebLogs", criteria);
    public Task<Result> AddWebLogAsync(LogInfo info) => Http.PostAsync("/Platform/AddWebLog", info);
    public Task<Result> DeleteWebLogsAsync(List<LogInfo> infos) => Http.PostAsync("/Platform/DeleteWebLogs", infos);
    public Task<Result> ClearWebLogsAsync() => Http.PostAsync("/Platform/ClearWebLogs");
}

partial class PlatformService
{
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