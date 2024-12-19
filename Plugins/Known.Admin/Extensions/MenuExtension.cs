namespace Known.Extensions;

static class MenuExtension
{
    internal static async Task<List<MenuInfo>> GetUserMenusAsync(this Database db)
    {
        var user = db.User;
        if (user == null)
            return [];

        var modules = AppData.Modules;
        if (modules == null || modules.Count == 0)
        {
            var items = await db.QueryListAsync<SysModule>();
            if (items != null && items.Count > 0)
            {
                modules = new List<ModuleInfo>();
                foreach (var item in items)
                {
                    modules.Add(item.ToModuleInfo());
                }
                AppData.Initialize(modules);
            }
        }

        var moduleIds = await db.GetRoleModuleIdsAsync(user.Id);
        return MenuHelper.GetUserMenus(user, modules, moduleIds);
    }
}