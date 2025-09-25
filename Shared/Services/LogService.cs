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
}

[Client]
class LogClient(HttpClient http) : ClientBase(http), ILogService
{
    public Task<PagingResult<LogInfo>> QueryLogsAsync(PagingCriteria criteria) => Http.QueryAsync<LogInfo>("/Log/QueryLogs", criteria);
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
}