namespace Known.Services;

/// <summary>
/// WebApi服务接口。
/// </summary>
public interface IWebApiService : IService
{
    /// <summary>
    /// 异步分页查询WebApi信息。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<ApiMethodInfo>> QueryWebApisAsync(PagingCriteria criteria);
}

[Client]
partial class WebApiClient(HttpClient http) : ClientBase(http), IWebApiService
{
    public Task<PagingResult<ApiMethodInfo>> QueryWebApisAsync(PagingCriteria criteria) => Http.QueryAsync<ApiMethodInfo>("/WebApi/QueryWebApis", criteria);
}

[WebApi, Service]
partial class WebApiService(Context context) : ServiceBase(context), IWebApiService
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
}