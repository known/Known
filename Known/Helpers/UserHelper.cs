namespace Known.Helpers;

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

    internal static async Task<List<MenuInfo>> GetUserMenusAsync(Database db, List<SysModule> modules)
    {
        var user = db.User;
        if (user == null)
            return [];

        if (user.IsAdmin)
            return modules.ToMenus(true);

        //var moduleIds = await DataRepository.GetUserModuleIdsAsync(db, user.Id);
        var sql = @"select a.ModuleId from SysRoleModule a 
where a.RoleId in (select RoleId from SysUserRole where UserId=@userId)
  and exists (select 1 from SysRole where Id=a.RoleId and Enabled='True')";
        var moduleIds = await db.ScalarsAsync<string>(sql, new { user.Id });
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
        var buttons = module.GetButtons();
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
        var actions = module.GetActions();
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
        var columns = module.Page?.Columns;
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