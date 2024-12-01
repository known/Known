namespace Known.Extensions;

static class MenuExtension
{
    internal static async Task<List<MenuInfo>> GetUserMenusAsync(this Database db)
    {
        var user = db.User;
        if (user == null)
            return [];

        var modules = DataHelper.Modules;
        if (modules == null || modules.Count == 0)
        {
            modules = await db.Query<SysModule>().ToListAsync<ModuleInfo>();
            DataHelper.Initialize(modules);
        }

        var routes = DataHelper.GetRouteModules(db.Context.Language, modules.Select(m => m.Url).ToList());
        if (routes != null && routes.Count > 0)
            modules.AddRange(routes);

        if (user.IsAdmin())
            return modules.ToMenus(true);

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

            AddParentModule(modules, userModules, item);
            item.Buttons = GetUserButtons(moduleIds, item);
            item.Actions = GetUserActions(moduleIds, item);
            item.Columns = GetUserColumns(moduleIds, item);
            userModules.Add(item);
        }
        return userModules.ToMenus(false);
    }

    private static void AddParentModule(List<ModuleInfo> modules, List<ModuleInfo> userModules, ModuleInfo item)
    {
        if (!userModules.Exists(m => m.Id == item.ParentId))
        {
            var parent = modules.FirstOrDefault(m => m.Id == item.ParentId);
            if (parent != null)
            {
                userModules.Add(parent);
                AddParentModule(modules, userModules, parent);
            }
        }
    }

    private static List<string> GetUserButtons(List<string> moduleIds, ModuleInfo module)
    {
        var buttons = module.GetToolButtons();
        if (buttons == null || buttons.Count == 0)
            return [];

        var datas = new List<string>();
        foreach (var item in buttons)
        {
            if (moduleIds.Contains($"b_{module.Id}_{item}"))
                datas.Add(item);
        }
        return datas;
    }

    private static List<string> GetUserActions(List<string> moduleIds, ModuleInfo module)
    {
        var actions = module.GetTableActions();
        if (actions == null || actions.Count == 0)
            return [];

        var datas = new List<string>();
        foreach (var item in actions)
        {
            if (moduleIds.Contains($"b_{module.Id}_{item}"))
                datas.Add(item);
        }
        return datas;
    }

    private static List<PageColumnInfo> GetUserColumns(List<string> moduleIds, ModuleInfo module)
    {
        var columns = module.GetPageColumns();
        if (columns == null || columns.Count == 0)
            return null;

        var datas = new List<PageColumnInfo>();
        foreach (var item in columns)
        {
            if (moduleIds.Contains($"c_{module.Id}_{item.Id}"))
                datas.Add(item);
        }
        return datas;
    }
}