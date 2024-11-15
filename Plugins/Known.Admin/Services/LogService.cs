namespace Known.Services;

/// <summary>
/// 系统日志服务接口。
/// </summary>
public interface ILogService : IService
{
    /// <summary>
    /// 异步分页查询系统日志。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<SysLog>> QueryLogsAsync(PagingCriteria criteria);
}

class LogService(Context context) : ServiceBase(context), ILogService
{
    public Task<PagingResult<SysLog>> QueryLogsAsync(PagingCriteria criteria)
    {
        if (criteria.OrderBys == null || criteria.OrderBys.Length == 0)
            criteria.OrderBys = [$"{nameof(SysLog.CreateTime)} desc"];
        return Database.QueryPageAsync<SysLog>(criteria);
    }
}