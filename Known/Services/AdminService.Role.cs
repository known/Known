namespace Known.Services;

public partial interface IAdminService
{
    /// <summary>
    /// 异步分页查询系统角色。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<RoleInfo>> QueryRolesAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步获取系统角色。
    /// </summary>
    /// <param name="roleId">角色ID。</param>
    /// <returns>系统角色。</returns>
    Task<RoleInfo> GetRoleAsync(string roleId);

    /// <summary>
    /// 异步删除系统角色。
    /// </summary>
    /// <param name="infos">系统角色列表。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteRolesAsync(List<RoleInfo> infos);

    /// <summary>
    /// 异步保存系统角色。
    /// </summary>
    /// <param name="info">系统角色信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveRoleAsync(RoleInfo info);
}

partial class AdminService
{
    private const string KeyRole = "Roles";

    public Task<PagingResult<RoleInfo>> QueryRolesAsync(PagingCriteria criteria)
    {
        return QueryModelsAsync<RoleInfo>(KeyRole, criteria);
    }

    public async Task<RoleInfo> GetRoleAsync(string roleId)
    {
        var datas = AppData.GetBizData<List<RoleInfo>>(KeyRole);
        var info = datas?.FirstOrDefault(d => d.Id == roleId) ?? new RoleInfo();
        info.Modules = await DataHelper.GetModulesAsync();
        return info;
    }

    public Task<Result> DeleteRolesAsync(List<RoleInfo> infos)
    {
        return DeleteModelsAsync(KeyRole, infos);
    }

    public Task<Result> SaveRoleAsync(RoleInfo info)
    {
        return SaveModelAsync(KeyRole, info);
    }
}

partial class AdminClient
{
    public Task<PagingResult<RoleInfo>> QueryRolesAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<RoleInfo>("/Admin/QueryRoles", criteria);
    }

    public Task<RoleInfo> GetRoleAsync(string roleId)
    {
        return Http.GetAsync<RoleInfo>($"/Admin/GetRole?roleId={roleId}");
    }

    public Task<Result> DeleteRolesAsync(List<RoleInfo> infos)
    {
        return Http.PostAsync("/Admin/DeleteRoles", infos);
    }

    public Task<Result> SaveRoleAsync(RoleInfo info)
    {
        return Http.PostAsync("/Admin/SaveRole", info);
    }
}