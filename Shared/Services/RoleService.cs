namespace Known.Services;

/// <summary>
/// 角色服务接口。
/// </summary>
public interface IRoleService : IService
{
    /// <summary>
    /// 异步分页查询角色信息。
    /// </summary>
    /// <param name="criteria">查询条件。</param>
    /// <returns></returns>
    Task<PagingResult<SysRole>> QueryRolesAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步获取角色信息。
    /// </summary>
    /// <param name="roleId">角色ID。</param>
    /// <returns></returns>
    Task<SysRole> GetRoleAsync(string roleId);

    /// <summary>
    /// 异步删除角色。
    /// </summary>
    /// <param name="infos">角色列表。</param>
    /// <returns></returns>
    Task<Result> DeleteRolesAsync(List<SysRole> infos);

    /// <summary>
    /// 异步保存角色。
    /// </summary>
    /// <param name="info">角色信息。</param>
    /// <returns></returns>
    Task<Result> SaveRoleAsync(SysRole info);
}

[Client]
class RoleClient(HttpClient http) : ClientBase(http), IRoleService
{
    public Task<PagingResult<SysRole>> QueryRolesAsync(PagingCriteria criteria) => Http.QueryAsync<SysRole>("/Role/QueryRoles", criteria);
    public Task<SysRole> GetRoleAsync(string roleId) => Http.GetAsync<SysRole>($"/Role/GetRole?roleId={roleId}");
    public Task<Result> DeleteRolesAsync(List<SysRole> infos) => Http.PostAsync("/Role/DeleteRoles", infos);
    public Task<Result> SaveRoleAsync(SysRole info) => Http.PostAsync("/Role/SaveRole", info);
}

[WebApi, Service]
class RoleService(Context context) : ServiceBase(context), IRoleService
{
    public Task<PagingResult<SysRole>> QueryRolesAsync(PagingCriteria criteria)
    {
        return Database.QueryPageAsync<SysRole>(criteria);
    }

    public async Task<SysRole> GetRoleAsync(string roleId)
    {
        SysRole info = null;
        await Database.QueryActionAsync(async db =>
        {
            info = string.IsNullOrWhiteSpace(roleId)
                 ? new SysRole()
                 : await db.QueryByIdAsync<SysRole>(roleId) ?? new SysRole();
            info.Menus = await DataHelper.GetMenusAsync(db);
            var roleModules = await db.QueryListAsync<SysRoleModule>(d => d.RoleId == roleId);
            info.MenuIds = roleModules?.Select(d => d.ModuleId).ToList();
        });
        return info;
    }

    public async Task<Result> DeleteRolesAsync(List<SysRole> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var database = Database;
        var result = await database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in infos)
            {
                await db.DeleteAsync<SysRole>(item.Id);
                await db.DeleteAsync<SysUserRole>(d => d.RoleId == item.Id);
                await db.DeleteAsync<SysRoleModule>(d => d.RoleId == item.Id);
            }
        });
        if (result.IsValid)
            database.UpdateUserRoleName();
        return result;
    }

    public async Task<Result> SaveRoleAsync(SysRole info)
    {
        var database = Database;
        var model = await database.QueryByIdAsync<SysRole>(info.Id);
        model ??= new SysRole();
        model.FillModel(info);

        var vr = model.Validate(Context);
        if (!vr.IsValid)
            return vr;

        var result = await database.TransactionAsync(Language.Save, async db =>
        {
            await db.SaveAsync(model);
            await db.DeleteAsync<SysRoleModule>(d => d.RoleId == model.Id);
            if (info.MenuIds != null && info.MenuIds.Count > 0)
            {
                foreach (var item in info.MenuIds)
                {
                    await db.InsertAsync(new SysRoleModule { RoleId = model.Id, ModuleId = item });
                }
            }
            info.Id = model.Id;
        }, info);
        if (result.IsValid)
            database.UpdateUserRoleName();
        return result;
    }
}