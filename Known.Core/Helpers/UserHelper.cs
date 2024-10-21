namespace Known.Core.Helpers;

class UserHelper
{
    internal static async Task<string> GetSystemNameAsync(Database db)
    {
        var sys = await SystemService.GetSystemAsync(db);
        var appName = sys?.AppName;
        if (string.IsNullOrWhiteSpace(appName))
            appName = Config.App.Name;
        return appName;
    }

    internal static async Task<UserInfo> GetUserByIdAsync(Database db, string id)
    {
        var user = await db.QueryAsync<SysUser>(d => d.Id == id);
        return Utils.MapTo<UserInfo>(user);
    }

    internal static async Task<UserInfo> GetUserByUserNameAsync(Database db, string userName)
    {
        var user = await db.QueryAsync<SysUser>(d => d.UserName == userName);
        return Utils.MapTo<UserInfo>(user);
    }

    internal static async Task<List<MenuInfo>> GetUserMenusAsync(Database db, List<SysModule> modules)
    {
        var user = db.User;
        if (user == null)
            return [];

        ModuleHelper.AddRouteModules(db.Context.Language, modules);
        
        if (user.IsAdmin())
            return modules.ToMenus(true);

        var moduleIds = await db.GetRoleModuleIdsAsync(user.Id);
        var userModules = new List<SysModule>();
        foreach (var item in modules)
        {
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

    private static void AddParentModule(List<SysModule> modules, List<SysModule> userModules, SysModule item)
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

    private static List<string> GetUserButtons(List<string> moduleIds, SysModule module)
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

    private static List<string> GetUserActions(List<string> moduleIds, SysModule module)
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

    private static List<PageColumnInfo> GetUserColumns(List<string> moduleIds, SysModule module)
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