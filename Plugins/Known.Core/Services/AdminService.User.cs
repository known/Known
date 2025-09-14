namespace Known.Services;

partial class AdminService
{
    public async Task<PagingResult<UserInfo>> QueryUsersAsync(PagingCriteria criteria)
    {
        //return await QueryUsersAsync<UserInfo>(criteria);TODO:为什么这里不能直接调用泛型方法？
        return await Database.QueryPageAsync<UserInfo>("SysUser", criteria);
    }
}