namespace Known.Services;

/// <summary>
/// 框架平台数据服务接口。
/// </summary>
public partial interface IPlatformService : IService
{
    /// <summary>
    /// 异步设置呈现模式。
    /// </summary>
    /// <param name="mode">呈现模式。</param>
    /// <returns></returns>
    Task<Result> SetRenderModeAsync(string mode);

    /// <summary>
    /// 异步分页查询WebApi信息。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<ApiMethodInfo>> QueryWebApisAsync(PagingCriteria criteria);
}

[Client]
partial class PlatformClient(HttpClient http) : ClientBase(http), IPlatformService
{
    public Task<Result> SetRenderModeAsync(string mode) => Http.PostAsync($"/Platform/SetRenderMode?mode={mode}");
    public Task<PagingResult<ApiMethodInfo>> QueryWebApisAsync(PagingCriteria criteria) => Http.QueryAsync<ApiMethodInfo>("/Platform/QueryWebApis", criteria);
}

[WebApi, Service]
partial class PlatformService(Context context) : ServiceBase(context), IPlatformService
{
    public Task<Result> SetRenderModeAsync(string mode)
    {
        Config.CurrentMode = Utils.ConvertTo<RenderType>(mode);
        return Result.SuccessAsync(Language.SetSuccess, Config.CurrentMode);
    }

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
}