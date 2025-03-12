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
    /// 异步分页查询系统用户。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<UserDataInfo>> QueryUserDatasAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步获取系统用户数据。
    /// </summary>
    /// <param name="id">用户ID。</param>
    /// <returns>系统用户数据。</returns>
    Task<UserDataInfo> GetUserDataAsync(string id);

    /// <summary>
    /// 异步删除系统用户。
    /// </summary>
    /// <param name="infos">系统用户列表。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteUsersAsync(List<UserDataInfo> infos);

    /// <summary>
    /// 异步切换系统用户所属部门。
    /// </summary>
    /// <param name="infos">系统用户列表。</param>
    /// <returns>切换结果。</returns>
    Task<Result> ChangeDepartmentAsync(List<UserDataInfo> infos);

    /// <summary>
    /// 异步启用系统用户。
    /// </summary>
    /// <param name="infos">系统用户列表。</param>
    /// <returns>启用结果。</returns>
    Task<Result> EnableUsersAsync(List<UserDataInfo> infos);

    /// <summary>
    /// 异步禁用系统用户。
    /// </summary>
    /// <param name="infos">系统用户列表。</param>
    /// <returns>禁用结果。</returns>
    Task<Result> DisableUsersAsync(List<UserDataInfo> infos);

    /// <summary>
    /// 异步重置系统用户密码。
    /// </summary>
    /// <param name="infos">系统用户列表。</param>
    /// <returns>重置结果。</returns>
    Task<Result> SetUserPwdsAsync(List<UserDataInfo> infos);

    /// <summary>
    /// 异步保存系统用户。
    /// </summary>
    /// <param name="info">系统用户信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveUserAsync(UserDataInfo info);
}

partial class AdminService
{
    private const string KeyUser = "Users";

    public Task<PagingResult<UserInfo>> QueryUsersAsync(PagingCriteria criteria)
    {
        return QueryModelsAsync<UserInfo>(KeyUser, criteria);
    }

    public Task<PagingResult<UserDataInfo>> QueryUserDatasAsync(PagingCriteria criteria)
    {
        return QueryModelsAsync<UserDataInfo>(KeyUser, criteria);
    }

    public Task<UserDataInfo> GetUserDataAsync(string id)
    {
        var info = AppData.GetBizData<List<UserDataInfo>>(KeyUser)?.FirstOrDefault(d => d.Id == id);
        info ??= new UserDataInfo();
        var roles = AppData.GetBizData<List<RoleInfo>>(KeyRole);
        info.Roles = roles?.Select(r => new CodeInfo(r.Id, r.Name)).ToList();
        return Task.FromResult(info);
    }

    public Task<Result> DeleteUsersAsync(List<UserDataInfo> infos)
    {
        return DeleteModelsAsync(KeyUser, infos);
    }

    public Task<Result> ChangeDepartmentAsync(List<UserDataInfo> infos)
    {
        return Result.SuccessAsync("更改成功！");
    }

    public Task<Result> EnableUsersAsync(List<UserDataInfo> infos)
    {
        return Result.SuccessAsync("启用成功！");
    }

    public Task<Result> DisableUsersAsync(List<UserDataInfo> infos)
    {
        return Result.SuccessAsync("禁用成功！");
    }

    public Task<Result> SetUserPwdsAsync(List<UserDataInfo> infos)
    {
        return Result.SuccessAsync("设置成功！");
    }

    public Task<Result> SaveUserAsync(UserDataInfo info)
    {
        return SaveModelAsync(KeyUser, info);
    }
}

partial class AdminClient
{
    public Task<PagingResult<UserInfo>> QueryUsersAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<UserInfo>("/Admin/QueryUsers", criteria);

    }
    public Task<PagingResult<UserDataInfo>> QueryUserDatasAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<UserDataInfo>("/Admin/QueryUserDatas", criteria);
    }

    public Task<UserDataInfo> GetUserDataAsync(string id)
    {
        return Http.GetAsync<UserDataInfo>($"/Admin/GetUserData?id={id}");
    }

    public Task<Result> DeleteUsersAsync(List<UserDataInfo> infos)
    {
        return Http.PostAsync("/Admin/DeleteUsers", infos);
    }

    public Task<Result> ChangeDepartmentAsync(List<UserDataInfo> infos)
    {
        return Http.PostAsync("/Admin/ChangeDepartment", infos);
    }

    public Task<Result> EnableUsersAsync(List<UserDataInfo> infos)
    {
        return Http.PostAsync("/Admin/EnableUsers", infos);
    }

    public Task<Result> DisableUsersAsync(List<UserDataInfo> infos)
    {
        return Http.PostAsync("/Admin/DisableUsers", infos);
    }

    public Task<Result> SetUserPwdsAsync(List<UserDataInfo> infos)
    {
        return Http.PostAsync("/Admin/SetUserPwds", infos);
    }

    public Task<Result> SaveUserAsync(UserDataInfo info)
    {
        return Http.PostAsync("/Admin/SaveUser", info);
    }
}