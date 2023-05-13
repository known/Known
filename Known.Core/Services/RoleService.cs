namespace Known.Core.Services;

class RoleService : BaseService
{
    internal RoleService(Context context) : base(context) { }

    internal PagingResult<SysRole> QueryRoles(PagingCriteria criteria)
    {
        return RoleRepository.QueryRoles(Database, criteria);
    }

    internal Result DeleteRoles(List<SysRole> entities)
    {
        if (entities == null || entities.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return Database.Transaction(Language.Delete, db =>
        {
            foreach (var item in entities)
            {
                db.Delete(item);
                RoleRepository.DeleteRoleUsers(db, item.Id);
                RoleRepository.DeleteRoleModules(db, item.Id);
            }
        });
    }

    internal RoleFormInfo GetRole(string roleId)
    {
        var info = new RoleFormInfo();
        var modules = ModuleRepository.GetModules(Database);
        info.Menus = modules.ToMenus();
        info.MenuIds = RoleRepository.GetRoleModuleIds(Database, roleId);
        return info;
    }

    internal Result SaveRole(RoleFormInfo info)
    {
        var entity = Database.QueryById<SysRole>((string)info.Model.Id);
        entity ??= new SysRole();
        entity.FillModel(info.Model);
        var vr = entity.Validate();
        if (!vr.IsValid)
            return vr;

        return Database.Transaction(Language.Save, db =>
        {
            db.Save(entity);
            RoleRepository.DeleteRoleModules(db, entity.Id);
            if (info.MenuIds != null && info.MenuIds.Count > 0)
            {
                foreach (var item in info.MenuIds)
                {
                    RoleRepository.AddRoleModule(db, entity.Id, item);
                }
            }
        }, entity.Id);
    }
}