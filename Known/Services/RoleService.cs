namespace Known.Services;

public interface IRoleService : IService
{
    Task<PagingResult<SysRole>> QueryRolesAsync(PagingCriteria criteria);
    Task<SysRole> GetRoleAsync(string roleId);
    Task<Result> DeleteRolesAsync(List<SysRole> models);
    Task<Result> SaveRoleAsync(SysRole model);
}

class RoleService(Context context) : ServiceBase(context), IRoleService
{
    public Task<PagingResult<SysRole>> QueryRolesAsync(PagingCriteria criteria)
    {
        return Database.QueryPageAsync<SysRole>(criteria);
    }

    public async Task<Result> DeleteRolesAsync(List<SysRole> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in models)
            {
                await db.DeleteAsync(item);
                await db.DeleteAsync<SysUserRole>(d => d.RoleId == item.Id);
                await db.DeleteAsync<SysRoleModule>(d => d.RoleId == item.Id);
            }
        });
    }

    public async Task<SysRole> GetRoleAsync(string roleId)
    {
        var db = Database;
        await db.OpenAsync();
        var info = string.IsNullOrWhiteSpace(roleId)
                 ? new SysRole()
                 : await db.QueryByIdAsync<SysRole>(roleId);
        info ??= new SysRole();
        info.Modules = await Repository.GetModulesAsync(db);
        var roleModules = await db.QueryListAsync<SysRoleModule>(d => d.RoleId == roleId);
        info.MenuIds = roleModules?.Select(d => d.ModuleId).ToList();
        await db.CloseAsync();
        return info;
    }

    public async Task<Result> SaveRoleAsync(SysRole model)
    {
        var vr = model.Validate(Context);
        if (!vr.IsValid)
            return vr;

        return await Database.TransactionAsync(Language.Save, async db =>
        {
            await db.SaveAsync(model);
            await db.DeleteAsync<SysRoleModule>(d => d.RoleId == model.Id);
            if (model.MenuIds != null && model.MenuIds.Count > 0)
            {
                foreach (var item in model.MenuIds)
                {
                    await db.InsertDataAsync(new SysRoleModule { RoleId = model.Id, ModuleId = item });
                }
            }
        }, model);
    }
}