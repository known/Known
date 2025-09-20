namespace Known.Services;

public interface IRoleService : IService
{
    Task<PagingResult<RoleInfo>> QueryRolesAsync(PagingCriteria criteria);
    Task<RoleInfo> GetRoleAsync(string roleId);
    Task<Result> DeleteRolesAsync(List<RoleInfo> infos);
    Task<Result> SaveRoleAsync(RoleInfo info);
}

[Client]
class RoleClient(HttpClient http) : ClientBase(http), IRoleService
{
    public Task<PagingResult<RoleInfo>> QueryRolesAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<RoleInfo>("/Role/QueryRoles", criteria);
    }

    public Task<RoleInfo> GetRoleAsync(string roleId)
    {
        return Http.GetAsync<RoleInfo>($"/Role/GetRole?roleId={roleId}");
    }

    public Task<Result> DeleteRolesAsync(List<RoleInfo> infos)
    {
        return Http.PostAsync("/Role/DeleteRoles", infos);
    }

    public Task<Result> SaveRoleAsync(RoleInfo info)
    {
        return Http.PostAsync("/Role/SaveRole", info);
    }
}

[WebApi, Service]
class RoleService(Context context) : ServiceBase(context), IRoleService
{
    public Task<PagingResult<RoleInfo>> QueryRolesAsync(PagingCriteria criteria)
    {
        return Database.Query<SysRole>(criteria).ToPageAsync<RoleInfo>();
    }

    public async Task<RoleInfo> GetRoleAsync(string roleId)
    {
        RoleInfo info = null;
        await Database.QueryActionAsync(async db =>
        {
            info = string.IsNullOrWhiteSpace(roleId)
                 ? new RoleInfo()
                 : await db.Query<SysRole>().Where(d => d.Id == roleId).FirstAsync<RoleInfo>();
            info ??= new RoleInfo();
            info.Menus = await DataHelper.GetMenusAsync(db);
            var roleModules = await db.QueryListAsync<SysRoleModule>(d => d.RoleId == roleId);
            info.MenuIds = roleModules?.Select(d => d.ModuleId).ToList();
        });
        return info;
    }

    public async Task<Result> DeleteRolesAsync(List<RoleInfo> infos)
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

    public async Task<Result> SaveRoleAsync(RoleInfo info)
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