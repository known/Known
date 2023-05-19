namespace Known.Clients;

public class LogClient : ClientBase
{
    public LogClient(Context context) : base(context) { }

    public Task<PagingResult<SysLog>> QueryLogsAsync(PagingCriteria criteria) => Context.QueryAsync<SysLog>("Log/QueryLogs", criteria);
    public Task<Result> AddLogAsync(SysLog log) => Context.PostAsync("Log/AddLog", log);
}