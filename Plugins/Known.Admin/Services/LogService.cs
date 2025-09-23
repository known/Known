namespace Known.Services;

public interface ILogService : IService
{
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