namespace Known.Services;

partial class AdminService
{
    public Task<Result> AddLogAsync(LogInfo log)
    {
        return Database.AddLogAsync(log);
    }

    public Task<PagingResult<SysLog>> QueryLogsAsync(PagingCriteria criteria)
    {
        if (criteria.OrderBys == null || criteria.OrderBys.Length == 0)
            criteria.OrderBys = [$"{nameof(SysLog.CreateTime)} desc"];
        return Database.QueryPageAsync<SysLog>(criteria);
    }
}