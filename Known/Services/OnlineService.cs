namespace Known.Services;

/// <summary>
/// 在线信息服务接口。
/// </summary>
public interface IOnlineService : IService
{
    /// <summary>
    /// 异步分页查询在线用户信息列表。
    /// </summary>
    /// <param name="criteria">查询条件。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<UserInfo>> QueryOnlineUsersAsync(PagingCriteria criteria);
}

[Client]
class OnlineClient(HttpClient http) : ClientBase(http), IOnlineService
{
    public Task<PagingResult<UserInfo>> QueryOnlineUsersAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<UserInfo>("/Online/QueryOnlineUsers", criteria);
    }
}

[WebApi, Service]
class OnlineService(Context context) : ServiceBase(context), IOnlineService
{
    public Task<PagingResult<UserInfo>> QueryOnlineUsersAsync(PagingCriteria criteria)
    {
        var datas = Cache.Users.ToList();
        var result = datas.ToPagingResult(criteria);
        return Task.FromResult(result);
    }
}