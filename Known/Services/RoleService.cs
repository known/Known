namespace Known.Services;

public interface IRoleService : IService
{
    Task<SysRole> GetRoleAsync(string roleId);
}

class RoleService(Context context) : ServiceBase(context), IRoleService
{
    public Task<PagingResult<SysRole>> QueryRolesAsync(PagingCriteria criteria)
    {
        return RoleRepository.QueryRolesAsync(Database, criteria);
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
                await RoleRepository.DeleteRoleUsersAsync(db, item.Id);
                await RoleRepository.DeleteRoleModulesAsync(db, item.Id);
            }
        });
    }

    public async Task<SysRole> GetRoleAsync(string roleId)
    {
        var info = await Database.QueryByIdAsync<SysRole>(roleId);
        info ??= new SysRole();
        var modules = await ModuleRepository.GetModulesAsync(Database);
        info.Menus = modules?.ToMenuItems(false);
        var menuIds = await RoleRepository.GetRoleModuleIdsAsync(Database, roleId);
        info.MenuIds = menuIds;
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
            await RoleRepository.DeleteRoleModulesAsync(db, model.Id);
            if (model.MenuIds != null && model.MenuIds.Count > 0)
            {
                foreach (var item in model.MenuIds)
                {
                    await RoleRepository.AddRoleModuleAsync(db, model.Id, item);
                }
            }
        }, model);
    }
}