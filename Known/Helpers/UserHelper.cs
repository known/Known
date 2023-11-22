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
            appName = Config.App.Name;
        return appName;
    }

    //internal static async Task<SettingInfo> GetUserSettingAsync(Database db)
    //{
    //    await db.OpenAsync();
    //    var info = await GetSettingByUserAsync<SettingInfo>(db, SettingInfo.KeyInfo);
    //    if (info != null)
    //    {
    //        var querys = await GetSettingsByUserAsync(db, SettingInfo.KeyQuery);
    //        info.Querys = querys.ToDictionary(s => s.BizName, s => s.DataAs<List<QueryInfo>>());
    //        //var columns = await GetSettingsByUserAsync(db, SettingInfo.KeyColumn);
    //        //info.Columns = columns.ToDictionary(s => s.BizName, s => s.DataAs<List<ColumnInfo>>());
    //    }
    //    await db.CloseAsync();
    //    return info;
    //}

    //private static async Task<List<SysSetting>> GetSettingsByUserAsync(Database db, string bizType) => await SettingRepository.GetSettingsByUserAsync(db, bizType);
    private static async Task<SysSetting> GetSettingByUserAsync(Database db, string bizType) => await SettingRepository.GetSettingByUserAsync(db, bizType) ?? new SysSetting { BizType = bizType };
    //private static async Task<T> GetSettingByUserAsync<T>(Database db, string bizType)
    //{
    //    var setting = await GetSettingByUserAsync(db, bizType);
    //    return setting.DataAs<T>();
    //}

    internal static async Task<List<MenuInfo>> GetUserMenusAsync(Database db)
    {
        var user = db.User;
        if (user == null)
            return [];

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

            item.Buttons = GetUserButtons(moduleIds, item);
            item.Actions = GetUserActions(moduleIds, item);
            item.Columns = GetUserColumns(moduleIds, item);
            userModules.Add(item);
        }
        return userModules.ToMenus();
    }

    private static List<string> GetUserButtons(List<string> moduleIds, SysModule module)
    {
        Config.PageButtons.TryGetValue(module.Code, out List<string> buttons);
        if (buttons == null || buttons.Count == 0)
            return null;

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
        Config.PageActions.TryGetValue(module.Code, out List<string> actions);
        if (actions == null || actions.Count == 0)
            return null;

        var datas = new List<string>();
        foreach (var item in actions)
        {
            if (moduleIds.Contains($"b_{module.Id}_{item}"))
                datas.Add(item);
        }
        return datas;
    }

    private static List<ColumnInfo> GetUserColumns(List<string> moduleIds, SysModule module)
    {
        var columns = Utils.FromJson<List<ColumnInfo>>(module.ColumnData);
        if (columns == null || columns.Count == 0)
            return null;

        var datas = new List<ColumnInfo>();
        foreach (var item in columns)
        {
            if (moduleIds.Contains($"c_{module.Id}_{item.Id}"))
                datas.Add(item);
        }
        return datas;
    }
}