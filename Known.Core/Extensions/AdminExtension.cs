﻿namespace Known.Core.Extensions;

static class AdminExtension
{
    internal static async Task<string> GetSystemNameAsync(this IAdminService service, Database db)
    {
        var sys = await service.GetSystemAsync(db);
        var appName = sys?.AppName;
        if (string.IsNullOrWhiteSpace(appName))
            appName = Config.App.Name;
        return appName;
    }

    internal static async Task<Result> CheckKeyAsync(this IAdminService service, Database db)
    {
        var info = await service.GetSystemAsync(db);
        return Config.App.CheckSystemInfo(info);
    }

    internal static async Task<T> GetUserSettingAsync<T>(this IAdminService service, Database db, string bizType)
    {
        var setting = await service.GetUserSettingAsync(db, bizType);
        if (setting == null)
            return default;

        return setting.DataAs<T>();
    }

    internal static async Task<Dictionary<string, List<TableSettingInfo>>> GetUserTableSettingsAsync(this IAdminService service, Database db)
    {
        var settings = await service.GetUserSettingsAsync(db, "UserTable_");
        if (settings == null || settings.Count == 0)
            return [];

        return settings.ToDictionary(k => k.BizType, v => v.DataAs<List<TableSettingInfo>>());
    }
}