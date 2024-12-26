namespace Known.Extensions;

static class MenuExtension
{
    internal static async Task<List<MenuInfo>> GetUserMenusAsync(this Database db)
    {
        var user = db.User;
        if (user == null)
            return [];

        var modules = AppData.Modules ?? [];
        if (modules.Count == 0)
        {
            // 从数据库中加载模块信息
            var items = await db.QueryListAsync<SysModule>();
            if (items != null && items.Count > 0)
            {
                foreach (var item in items.OrderBy(m => m.Sort))
                {
                    modules.Add(item.ToModuleInfo());
                }
                AppData.Initialize(modules);
            }
        }

        // 如果是管理员，返回所有菜单
        if (user.IsAdmin())
            return MenuHelper.GetUserMenus(user, modules);

        // 如果是角色用户，根据用户角色模块ID列表返回菜单
        var moduleIds = await db.GetRoleModuleIdsAsync(user.Id);
        var userModules = new List<ModuleInfo>();
        foreach (var item in modules)
        {
            if (!item.Enabled)
                continue;

            if (!moduleIds.Contains(item.Id))
                continue;

            if (userModules.Exists(m => m.Id == item.Id))
                continue;

            SetPluginPermission(item, moduleIds);
            userModules.Add(item);
        }

        return MenuHelper.GetUserMenus(user, userModules, moduleIds);
    }

    private static void SetPluginPermission(ModuleInfo module, List<string> moduleIds)
    {
        var plugin = module.GetPlugin<EntityPluginInfo>();
        if (plugin == null)
            return;

        if (plugin.Page != null)
        {
            if (plugin.Page.Tools != null)
                plugin.Page.Tools = GetUserButtons(plugin.Page.Tools, moduleIds, module);
            if (plugin.Page.Actions != null)
                plugin.Page.Actions = GetUserActions(plugin.Page.Actions, moduleIds, module);
            if (plugin.Page.Columns != null)
                plugin.Page.Columns = GetUserColumns(plugin.Page.Columns, moduleIds, module);
        }

        module.Plugins = [];
        module.AddPlugin(plugin);
    }

    private static string[] GetUserButtons(string[] buttons, List<string> moduleIds, ModuleInfo module)
    {
        var datas = new List<string>();
        foreach (var item in buttons)
        {
            if (moduleIds.Contains($"b_{module.Id}_{item}"))
                datas.Add(item);
        }
        return [.. datas];
    }

    private static string[] GetUserActions(string[] actions, List<string> moduleIds, ModuleInfo module)
    {
        var datas = new List<string>();
        foreach (var item in actions)
        {
            if (moduleIds.Contains($"b_{module.Id}_{item}"))
                datas.Add(item);
        }
        return [.. datas];
    }

    private static List<PageColumnInfo> GetUserColumns(List<PageColumnInfo> columns, List<string> moduleIds, ModuleInfo module)
    {
        var datas = new List<PageColumnInfo>();
        foreach (var item in columns)
        {
            if (moduleIds.Contains($"c_{module.Id}_{item.Id}"))
                datas.Add(item);
        }
        return datas;
    }
}