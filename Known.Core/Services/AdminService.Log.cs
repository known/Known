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

    public Task<PagingResult<LogInfo>> QueryWebLogsAsync(PagingCriteria criteria)
    {
        return Logger.QueryLogsAsync(criteria);
    }

    public Task<Result> AddWebLogAsync(LogInfo info)
    {
        Logger.Logs.Add(info);
        return Result.SuccessAsync("添加成功！");
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