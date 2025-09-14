namespace Known.Extensions;

static class MenuExtension
{
    internal static string ZipDataString(this List<PluginInfo> plugins)
    {
        if (plugins == null || plugins.Count == 0)
            return string.Empty;

        return ZipHelper.ZipDataAsString(plugins);
    }

    internal static async Task<List<MenuInfo>> GetUserMenusAsync(this Database db)
    {
        var user = db.User;
        if (user == null)
            return [];

        var modules = await DataHelper.GetModulesAsync(db);
        // 如果是管理员，返回所有菜单
        if (user.IsAdmin())
            return modules.ToMenus();

        // 如果是角色用户，根据用户角色模块ID列表返回菜单
        var moduleIds = await Config.OnRoleModule?.Invoke(db, user.Id);
        if (moduleIds == null || moduleIds.Count == 0)
            return [];

        var userModules = new List<ModuleInfo>();
        foreach (var item in modules)
        {
            if (!item.Enabled)
                continue;

            if (!moduleIds.Contains(item.Id))
                continue;

            if (userModules.Exists(m => m.Id == item.Id))
                continue;

            var module = item.Clone();
            AddParentModule(modules, userModules, module);
            SetPluginPermission(module, moduleIds);
            userModules.Add(module);
        }

        return userModules.ToMenus();
    }

    private static void AddParentModule(List<ModuleInfo> allModules, List<ModuleInfo> userModules, ModuleInfo item)
    {
        // 如果父模块不存在，则添加父模块
        if (!userModules.Exists(m => m.Id == item.ParentId))
        {
            var parent = allModules.FirstOrDefault(m => m.Id == item.ParentId);
            if (parent != null)
            {
                userModules.Add(parent);
                AddParentModule(allModules, userModules, parent);
            }
        }
    }

    private static void SetPluginPermission(ModuleInfo module, List<string> moduleIds)
    {
        var param = module?.Plugins?.GetPluginParameter<AutoPageInfo>();
        if (param == null)
            return;

        if (param.Page != null)
        {
            if (param.Page.Tools != null)
                param.Page.Tools = GetUserButtons(param.Page.Tools, moduleIds, module);
            if (param.Page.Actions != null)
                param.Page.Actions = GetUserActions(param.Page.Actions, moduleIds, module);
            if (param.Page.Columns != null)
                param.Page.Columns = GetUserColumns(param.Page.Columns, moduleIds, module);
        }

        module.Plugins.AddPlugin(param, param.Id, param.Type);
    }

    private static List<string> GetUserButtons(List<string> buttons, List<string> moduleIds, ModuleInfo module)
    {
        var datas = new List<string>();
        foreach (var item in buttons)
        {
            if (moduleIds.Contains($"b_{module.Id}_{item}"))
                datas.Add(item);
        }
        return datas;
    }

    private static List<string> GetUserActions(List<string> actions, List<string> moduleIds, ModuleInfo module)
    {
        var datas = new List<string>();
        foreach (var item in actions)
        {
            if (moduleIds.Contains($"b_{module.Id}_{item}"))
                datas.Add(item);
        }
        return datas;
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