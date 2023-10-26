namespace Known.Services;

public class RoleService : BaseService
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

    public async Task<RoleFormInfo> GetRoleAsync(string roleId)
    {
        var info = new RoleFormInfo();
        var modules = await ModuleRepository.GetModulesAsync(Database);
        info.Menus = modules.ToMenus();
        info.MenuIds = await RoleRepository.GetRoleModuleIdsAsync(Database, roleId);
        return info;
    }

    public async Task<Result> SaveRoleAsync(RoleFormInfo info)
    {
        var entity = await Database.QueryByIdAsync<SysRole>((string)info.Model.Id);
        entity ??= new SysRole();
        entity.FillModel(info.Model);
        var vr = entity.Validate();
        if (!vr.IsValid)
            return vr;

        return await Database.TransactionAsync(Language.Save, async db =>
        {
            await db.SaveAsync(entity);
            await RoleRepository.DeleteRoleModulesAsync(db, entity.Id);
            if (info.MenuIds != null && info.MenuIds.Count > 0)
            {
                foreach (var item in info.MenuIds)
                {
                    await RoleRepository.AddRoleModuleAsync(db, entity.Id, item);
                }
            }
        }, entity.Id);
    }
}