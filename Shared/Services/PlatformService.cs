namespace Known.Services;

/// <summary>
/// 框架平台数据服务接口。
/// </summary>
public partial interface IPlatformService : IService
{
    /// <summary>
    /// 异步分页查询WebApi信息。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<ApiMethodInfo>> QueryWebApisAsync(PagingCriteria criteria);

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
partial class PlatformClient(HttpClient http) : ClientBase(http), IPlatformService
{
    public Task<PagingResult<ApiMethodInfo>> QueryWebApisAsync(PagingCriteria criteria) => Http.QueryAsync<ApiMethodInfo>("/Platform/QueryWebApis", criteria);
    public Task<PagingResult<LogInfo>> QueryWebLogsAsync(PagingCriteria criteria) => Http.QueryAsync<LogInfo>("/Platform/QueryWebLogs", criteria);
    public Task<Result> AddWebLogAsync(LogInfo info) => Http.PostAsync("/Platform/AddWebLog", info);
    public Task<Result> DeleteWebLogsAsync(List<LogInfo> infos) => Http.PostAsync("/Platform/DeleteWebLogs", infos);
    public Task<Result> ClearWebLogsAsync() => Http.PostAsync("/Platform/ClearWebLogs");
}

[WebApi, Service]
partial class PlatformService(Context context) : ServiceBase(context), IPlatformService
{
    public Task<PagingResult<ApiMethodInfo>> QueryWebApisAsync(PagingCriteria criteria)
    {
        var methods = CoreConfig.ApiMethods;
        var method = criteria.GetQueryValue(nameof(ApiMethodInfo.HttpMethod));
        if (!string.IsNullOrWhiteSpace(method))
            methods = [.. methods.Where(m => m.HttpMethod.Method.Equals(method, StringComparison.OrdinalIgnoreCase))];
        methods = [.. methods.Contains(m => m.Route, criteria)];
        var result = methods.ToPagingResult(criteria);
        return Task.FromResult(result);
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