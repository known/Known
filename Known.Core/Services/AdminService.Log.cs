namespace Known.Services;

partial class AdminService
{
    public Task<Result> AddLogAsync(LogInfo info)
    {
        return Database.AddLogAsync(info);
    }

    public Task<PagingResult<LogInfo>> QueryLogsAsync(PagingCriteria criteria)
    {
        if (criteria.OrderBys == null || criteria.OrderBys.Length == 0)
            criteria.OrderBys = [$"{nameof(LogInfo.CreateTime)} desc"];
        return Database.Query<SysLog>(criteria).ToPageAsync<LogInfo>();
    }
}