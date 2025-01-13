namespace Known.Services;

public partial interface IAdminService
{
    /// <summary>
    /// 异步分页查询系统用户。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<UserInfo>> QueryUsersAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步获取系统用户数据。
    /// </summary>
    /// <param name="id">用户ID。</param>
    /// <returns>系统用户数据。</returns>
    Task<UserInfo> GetUserDataAsync(string id);

    /// <summary>
    /// 异步删除系统用户。
    /// </summary>
    /// <param name="infos">系统用户列表。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteUsersAsync(List<UserInfo> infos);

    /// <summary>
    /// 异步切换系统用户所属部门。
    /// </summary>
    /// <param name="infos">系统用户列表。</param>
    /// <returns>切换结果。</returns>
    Task<Result> ChangeDepartmentAsync(List<UserInfo> infos);

    /// <summary>
    /// 异步启用系统用户。
    /// </summary>
    /// <param name="infos">系统用户列表。</param>
    /// <returns>启用结果。</returns>
    Task<Result> EnableUsersAsync(List<UserInfo> infos);

    /// <summary>
    /// 异步禁用系统用户。
    /// </summary>
    /// <param name="infos">系统用户列表。</param>
    /// <returns>禁用结果。</returns>
    Task<Result> DisableUsersAsync(List<UserInfo> infos);

    /// <summary>
    /// 异步重置系统用户密码。
    /// </summary>
    /// <param name="infos">系统用户列表。</param>
    /// <returns>重置结果。</returns>
    Task<Result> SetUserPwdsAsync(List<UserInfo> infos);

    /// <summary>
    /// 异步保存系统用户。
    /// </summary>
    /// <param name="info">系统用户信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveUserAsync(UserInfo info);
}

partial class AdminService
{
    public Task<PagingResult<UserInfo>> QueryUsersAsync(PagingCriteria criteria)
    {
        return Task.FromResult(new PagingResult<UserInfo>());
    }

    public Task<UserInfo> GetUserDataAsync(string id)
    {
        return Task.FromResult(new UserInfo());
    }

    public Task<Result> DeleteUsersAsync(List<UserInfo> infos)
    {
        return Result.SuccessAsync("删除成功！");
    }

    public Task<Result> ChangeDepartmentAsync(List<UserInfo> infos)
    {
        return Result.SuccessAsync("更改成功！");
    }

    public Task<Result> EnableUsersAsync(List<UserInfo> infos)
    {
        return Result.SuccessAsync("启用成功！");
    }

    public Task<Result> DisableUsersAsync(List<UserInfo> infos)
    {
        return Result.SuccessAsync("禁用成功！");
    }

    public Task<Result> SetUserPwdsAsync(List<UserInfo> infos)
    {
        return Result.SuccessAsync("设置成功！");
    }

    public Task<Result> SaveUserAsync(UserInfo info)
    {
        return Result.SuccessAsync("保存成功！");
    }
}

partial class AdminClient
{
    public Task<PagingResult<UserInfo>> QueryUsersAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<UserInfo>("/Admin/QueryUsers", criteria);
    }

    public Task<UserInfo> GetUserDataAsync(string id)
    {
        return Http.GetAsync<UserInfo>($"/Admin/GetUserData?id={id}");
    }

    public Task<Result> DeleteUsersAsync(List<UserInfo> infos)
    {
        return Http.PostAsync("/Admin/DeleteUsers", infos);
    }

    public Task<Result> ChangeDepartmentAsync(List<UserInfo> infos)
    {
        return Http.PostAsync("/Admin/ChangeDepartment", infos);
    }

    public Task<Result> EnableUsersAsync(List<UserInfo> infos)
    {
        return Http.PostAsync("/Admin/EnableUsers", infos);
    }

    public Task<Result> DisableUsersAsync(List<UserInfo> infos)
    {
        return Http.PostAsync("/Admin/DisableUsers", infos);
    }

    public Task<Result> SetUserPwdsAsync(List<UserInfo> infos)
    {
        return Http.PostAsync("/Admin/SetUserPwds", infos);
    }

    public Task<Result> SaveUserAsync(UserInfo info)
    {
        return Http.PostAsync("/Admin/SaveUser", info);
    }
}