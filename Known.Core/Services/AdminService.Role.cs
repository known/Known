namespace Known.Services;

partial class AdminService
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
        var database = Database;
        await database.OpenAsync();
        var info = string.IsNullOrWhiteSpace(roleId)
                 ? new SysRole()
                 : await database.QueryByIdAsync<SysRole>(roleId);
        info ??= new SysRole();
        info.Modules = AppData.Modules;
        var routes = DataHelper.GetRouteModules(info.Modules.Select(m => m.Url).ToList());
        if (routes != null && routes.Count > 0)
        {
            info.Modules.AddRange(routes.Select(r =>
            {
                var param = r.Plugins?.GetPluginParameter<TablePageInfo>();
                var module = new ModuleInfo
                {
                    Id = r.Id,
                    ParentId = r.ParentId,
                    Name = r.Name,
                    Url = r.Url,
                    Icon = r.Icon,
                    Target = r.Target,
                    Enabled = r.Enabled,
                    //Page = param?.Page
                };
                return module;
            }));
        }
        info.Modules = info.Modules.Where(m => m.Enabled).ToList();
        var roleModules = await database.QueryListAsync<SysRoleModule>(d => d.RoleId == roleId);
        info.MenuIds = roleModules?.Select(d => d.ModuleId).ToList();
        await database.CloseAsync();
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
                    await db.InsertAsync(new SysRoleModule { RoleId = model.Id, ModuleId = item });
                }
            }
        }, model);
    }
}