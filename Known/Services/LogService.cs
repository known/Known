namespace Known.Services;

/// <summary>
/// 日志服务接口。
/// </summary>
public interface ILogService : IService
{
    /// <summary>
    /// 异步分页查询日志信息。
    /// </summary>
    /// <param name="criteria">查询条件。</param>
    /// <returns></returns>
    Task<PagingResult<LogInfo>> QueryLogsAsync(PagingCriteria criteria);

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

[Client]
class LogClient(HttpClient http) : ClientBase(http), ILogService
{
    public Task<PagingResult<LogInfo>> QueryLogsAsync(PagingCriteria criteria) => Http.QueryAsync<LogInfo>("/Log/QueryLogs", criteria);
    public Task<PagingResult<LogInfo>> QueryWebLogsAsync(PagingCriteria criteria) => Http.QueryAsync<LogInfo>("/Log/QueryWebLogs", criteria);
    public Task<Result> AddWebLogAsync(LogInfo info) => Http.PostAsync("/Log/AddWebLog", info);
    public Task<Result> DeleteWebLogsAsync(List<LogInfo> infos) => Http.PostAsync("/Log/DeleteWebLogs", infos);
    public Task<Result> ClearWebLogsAsync() => Http.PostAsync("/Log/ClearWebLogs");
}

[WebApi, Service]
class LogService(Context context) : ServiceBase(context), ILogService
{
    public Task<PagingResult<LogInfo>> QueryLogsAsync(PagingCriteria criteria)
    {
        if (criteria.OrderBys == null || criteria.OrderBys.Length == 0)
            criteria.OrderBys = [$"{nameof(LogInfo.CreateTime)} desc"];
        return Database.Query<SysLog>(criteria).ToPageAsync<LogInfo>();
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