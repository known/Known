namespace Known.Services;

public partial interface IAdminService
{
    /// <summary>
    /// 异步分页查询系统用户。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<SysUser>> QueryUsersAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步获取系统用户数据。
    /// </summary>
    /// <param name="id">用户ID。</param>
    /// <returns>系统用户数据。</returns>
    Task<SysUser> GetUserDataAsync(string id);

    /// <summary>
    /// 异步删除系统用户。
    /// </summary>
    /// <param name="models">系统用户列表。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteUsersAsync(List<SysUser> models);

    /// <summary>
    /// 异步切换系统用户所属部门。
    /// </summary>
    /// <param name="models">系统用户列表。</param>
    /// <returns>切换结果。</returns>
    Task<Result> ChangeDepartmentAsync(List<SysUser> models);

    /// <summary>
    /// 异步启用系统用户。
    /// </summary>
    /// <param name="models">系统用户列表。</param>
    /// <returns>启用结果。</returns>
    Task<Result> EnableUsersAsync(List<SysUser> models);

    /// <summary>
    /// 异步禁用系统用户。
    /// </summary>
    /// <param name="models">系统用户列表。</param>
    /// <returns>禁用结果。</returns>
    Task<Result> DisableUsersAsync(List<SysUser> models);

    /// <summary>
    /// 异步重置系统用户密码。
    /// </summary>
    /// <param name="models">系统用户列表。</param>
    /// <returns>重置结果。</returns>
    Task<Result> SetUserPwdsAsync(List<SysUser> models);

    /// <summary>
    /// 异步保存系统用户。
    /// </summary>
    /// <param name="model">系统用户信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveUserAsync(SysUser model);
}

partial class AdminService
{
    public Task<PagingResult<SysUser>> QueryUsersAsync(PagingCriteria criteria)
    {
        return Task.FromResult(new PagingResult<SysUser>());
    }

    public Task<SysUser> GetUserDataAsync(string id)
    {
        return Task.FromResult(new SysUser());
    }

    public Task<Result> DeleteUsersAsync(List<SysUser> models)
    {
        return Result.SuccessAsync("删除成功！");
    }

    public Task<Result> ChangeDepartmentAsync(List<SysUser> models)
    {
        return Result.SuccessAsync("更改成功！");
    }

    public Task<Result> EnableUsersAsync(List<SysUser> models)
    {
        return Result.SuccessAsync("启用成功！");
    }

    public Task<Result> DisableUsersAsync(List<SysUser> models)
    {
        return Result.SuccessAsync("禁用成功！");
    }

    public Task<Result> SetUserPwdsAsync(List<SysUser> models)
    {
        return Result.SuccessAsync("设置成功！");
    }

    public Task<Result> SaveUserAsync(SysUser model)
    {
        return Result.SuccessAsync("保存成功！");
    }
}

partial class AdminClient
{
    public Task<PagingResult<SysUser>> QueryUsersAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<SysUser>("/Admin/QueryUsers", criteria);
    }

    public Task<SysUser> GetUserDataAsync(string id)
    {
        return Http.GetAsync<SysUser>($"/Admin/GetUserData?id={id}");
    }

    public Task<Result> DeleteUsersAsync(List<SysUser> models)
    {
        return Http.PostAsync("/Admin/DeleteUsers", models);
    }

    public Task<Result> ChangeDepartmentAsync(List<SysUser> models)
    {
        return Http.PostAsync("/Admin/ChangeDepartment", models);
    }

    public Task<Result> EnableUsersAsync(List<SysUser> models)
    {
        return Http.PostAsync("/Admin/EnableUsers", models);
    }

    public Task<Result> DisableUsersAsync(List<SysUser> models)
    {
        return Http.PostAsync("/Admin/DisableUsers", models);
    }

    public Task<Result> SetUserPwdsAsync(List<SysUser> models)
    {
        return Http.PostAsync("/Admin/SetUserPwds", models);
    }

    public Task<Result> SaveUserAsync(SysUser model)
    {
        return Http.PostAsync("/Admin/SaveUser", model);
    }
}