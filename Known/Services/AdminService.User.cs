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

partial class AdminClient
{
    public Task<PagingResult<UserInfo>> QueryUsersAsync(PagingCriteria criteria) => Http.QueryAsync<UserInfo>("/Admin/QueryUsers", criteria);
}

partial class AdminService
{
    public async Task<PagingResult<UserInfo>> QueryUsersAsync(PagingCriteria criteria)
    {
        return await Database.Query<SysUser>(criteria).ToPageAsync<UserInfo>();
    }
}