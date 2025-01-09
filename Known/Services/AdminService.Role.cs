namespace Known.Services;

public partial interface IAdminService
{
    /// <summary>
    /// 异步分页查询系统角色。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<SysRole>> QueryRolesAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步获取系统角色。
    /// </summary>
    /// <param name="roleId">角色ID。</param>
    /// <returns>系统角色。</returns>
    Task<SysRole> GetRoleAsync(string roleId);

    /// <summary>
    /// 异步删除系统角色。
    /// </summary>
    /// <param name="models">系统角色列表。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteRolesAsync(List<SysRole> models);

    /// <summary>
    /// 异步保存系统角色。
    /// </summary>
    /// <param name="model">系统角色信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveRoleAsync(SysRole model);
}

partial class AdminService
{
    public Task<PagingResult<SysRole>> QueryRolesAsync(PagingCriteria criteria)
    {
        return Task.FromResult(new PagingResult<SysRole>());
    }

    public Task<SysRole> GetRoleAsync(string roleId)
    {
        return Task.FromResult(new SysRole());
    }

    public Task<Result> DeleteRolesAsync(List<SysRole> models)
    {
        return Result.SuccessAsync("删除成功！");
    }

    public Task<Result> SaveRoleAsync(SysRole model)
    {
        return Result.SuccessAsync("保存成功！");
    }
}

partial class AdminClient
{
    public Task<PagingResult<SysRole>> QueryRolesAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<SysRole>("/Admin/QueryRoles", criteria);
    }

    public Task<SysRole> GetRoleAsync(string roleId)
    {
        return Http.GetAsync<SysRole>($"/Admin/GetRole?roleId={roleId}");
    }

    public Task<Result> DeleteRolesAsync(List<SysRole> models)
    {
        return Http.PostAsync("/Admin/DeleteRoles", models);
    }

    public Task<Result> SaveRoleAsync(SysRole model)
    {
        return Http.PostAsync("/Admin/SaveRole", model);
    }
}