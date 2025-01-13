namespace Known.Services;

partial class AdminService
{
    public Task<PagingResult<RoleInfo>> QueryRolesAsync(PagingCriteria criteria)
    {
        return Database.Query<SysRole>(criteria).ToPageAsync<RoleInfo>();
    }

    public async Task<Result> DeleteRolesAsync(List<RoleInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in infos)
            {
                await db.DeleteAsync<SysRole>(item.Id);
                await db.DeleteAsync<SysUserRole>(d => d.RoleId == item.Id);
                await db.DeleteAsync<SysRoleModule>(d => d.RoleId == item.Id);
            }
        });
    }

    public async Task<RoleInfo> GetRoleAsync(string roleId)
    {
        var database = Database;
        await database.OpenAsync();
        var info = string.IsNullOrWhiteSpace(roleId)
                 ? new RoleInfo()
                 : await database.Query<SysRole>().Where(d => d.Id == roleId).FirstAsync<RoleInfo>();
        info ??= new RoleInfo();
        info.Modules = DataHelper.GetRoleModules();
        var roleModules = await database.QueryListAsync<SysRoleModule>(d => d.RoleId == roleId);
        info.MenuIds = roleModules?.Select(d => d.ModuleId).ToList();
        await database.CloseAsync();
        return info;
    }

    public async Task<Result> SaveRoleAsync(RoleInfo info)
    {
        var database = Database;
        var model = await database.QueryByIdAsync<SysRole>(info.Id);
        model ??= new SysRole();
        model.Name = info.Name;
        model.Enabled = info.Enabled;
        model.Note = info.Note;

        var vr = model.Validate(Context);
        if (!vr.IsValid)
            return vr;

        return await database.TransactionAsync(Language.Save, async db =>
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
        }, info);
    }
}