using Known.Entities;
using Known.Extensions;
using Known.Repositories;
using Known.Services;

namespace Known.Helpers;

class UserHelper
{
    internal static async Task<string> GetSystemNameAsync(Database db)
    {
        var sys = await SystemService.GetSystemAsync(db);
        var appName = sys?.AppName;
        if (string.IsNullOrWhiteSpace(appName))
            appName = Config.AppName;
        return appName;
    }

    internal static async Task<UserSetting> GetUserSettingAsync(Database db)
    {
        await db.OpenAsync();
        var info = await GetSettingByUserAsync<SettingInfo>(db, UserSetting.KeyInfo);
        var querys = await GetSettingsByUserAsync(db, UserSetting.KeyQuery);
        var columns = await GetSettingsByUserAsync(db, UserSetting.KeyColumn);
        await db.CloseAsync();
        return new UserSetting
        {
            Info = info,
            Querys = querys.ToDictionary(s => s.BizName, s => s.DataAs<List<QueryInfo>>()),
            Columns = columns.ToDictionary(s => s.BizName, s => s.DataAs<List<ColumnInfo>>())
        };
    }

    private static async Task<List<SysSetting>> GetSettingsByUserAsync(Database db, string bizType) => await SettingRepository.GetSettingsByUserAsync(db, bizType);
    private static async Task<SysSetting> GetSettingByUserAsync(Database db, string bizType) => await SettingRepository.GetSettingByUserAsync(db, bizType) ?? new SysSetting { BizType = bizType };
    private static async Task<T> GetSettingByUserAsync<T>(Database db, string bizType)
    {
        var setting = await GetSettingByUserAsync(db, bizType);
        return setting.DataAs<T>();
    }

    internal static async Task<List<MenuInfo>> GetUserMenusAsync(Database db)
    {
        var user = db.User;
        if (user == null)
            return new List<MenuInfo>();

        var modules = await ModuleRepository.GetModulesAsync(db);
        if (user.IsAdmin)
            return modules.ToMenus();

        var moduleIds = await UserRepository.GetUserModuleIdsAsync(db, user.Id);
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