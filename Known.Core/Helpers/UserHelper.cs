namespace Known.Core.Helpers;

class UserHelper
{
    internal static string GetSystemName(Database db)
    {
        var sys = SystemService.GetSystem(db);
        var appName = sys?.AppName;
        if (string.IsNullOrWhiteSpace(appName))
            appName = Config.AppName;
        return appName;
    }

    internal static UserSetting GetUserSetting(Database db)
    {
        db.Open();
        var info = SettingService.GetSettingByUser<SettingInfo>(db, UserSetting.KeyInfo);
        var querys = SettingRepository.GetSettings(db, UserSetting.KeyQuery);
        var columns = SettingRepository.GetSettings(db, UserSetting.KeyColumn);
        db.Close();
        return new UserSetting
        {
            Info = info,
            Querys = querys.ToDictionary(s => s.BizName, s => s.DataAs<List<QueryInfo>>()),
            Columns = columns.ToDictionary(s => s.BizName, s => s.DataAs<List<ColumnInfo>>())
        };
    }

    internal static List<MenuInfo> GetUserMenus(Database db)
    {
        var user = db.User;
        if (user == null)
            return new List<MenuInfo>();

        var modules = ModuleRepository.GetModules(db);
        if (user.IsAdmin)
            return modules.ToMenus();

        var moduleIds = UserRepository.GetUserModuleIds(db, user.Id);
        var userModules = new List<SysModule>();
        foreach (var item in modules)
        {
            if (!moduleIds.Contains(item.Id))
                continue;

            if (userModules.Exists(m => m.Id == item.Id))
                continue;

            if (!userModules.Exists(m => m.Id == item.ParentId))
            {
                var parent = modules.FirstOrDefault(m => m.Id == item.ParentId);
                if (parent != null)
                    userModules.Add(parent);
            }

            item.ButtonData = GetUserButtonData(moduleIds, item);
            item.ActionData = GetUserActionData(moduleIds, item);
            item.ColumnData = GetUserColumnData(moduleIds, item);
            userModules.Add(item);
        }
        return userModules.ToMenus();
    }

    private static string GetUserButtonData(List<string> moduleIds, SysModule module)
    {
        if (module.Buttons == null || module.Buttons.Count == 0)
            return null;

        var buttons = new List<string>();
        foreach (var item in module.Buttons)
        {
            if (moduleIds.Contains($"b_{module.Id}_{item}"))
                buttons.Add(item);
        }

        if (buttons.Count == 0)
            return null;

        return string.Join(",", buttons);
    }

    private static string GetUserActionData(List<string> moduleIds, SysModule module)
    {
        if (module.Actions == null || module.Actions.Count == 0)
            return null;

        var actions = new List<string>();
        foreach (var item in module.Actions)
        {
            if (moduleIds.Contains($"b_{module.Id}_{item}"))
                actions.Add(item);
        }

        if (actions.Count == 0)
            return null;

        return string.Join(",", actions);
    }

    private static string GetUserColumnData(List<string> moduleIds, SysModule module)
    {
        if (module.Columns == null || module.Columns.Count == 0)
            return null;

        var columns = new List<ColumnInfo>();
        foreach (var item in module.Columns)
        {
            if (moduleIds.Contains($"c_{module.Id}_{item.Id}"))
                columns.Add(item);
        }

        if (columns.Count == 0)
            return null;

        return Utils.ToJson(columns);
    }
}