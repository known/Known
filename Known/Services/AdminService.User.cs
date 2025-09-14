namespace Known.Services;

public partial interface IAdminService
{
    /// <summary>
    /// 异步分页查询系统用户。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<UserInfo>> QueryUsersAsync(PagingCriteria criteria);
}

partial class AdminService
{
    private const string KeyUser = "Users";

    public Task<PagingResult<UserInfo>> QueryUsersAsync(PagingCriteria criteria)
    {
        return QueryModelsAsync<UserInfo>(KeyUser, criteria);
    }
}

partial class AdminClient
{
    public Task<PagingResult<UserInfo>> QueryUsersAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<UserInfo>("/Admin/QueryUsers", criteria);
    }
}