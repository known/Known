namespace Known.Extensions;

static class SettingExtension
{
    internal static Task<List<SettingInfo>> GetUserSettingsAsync(this Database db, string bizTypePrefix)
    {
        return db.Query<SysSetting>()
                 .Where(d => d.CreateBy == db.UserName && d.BizType.StartsWith(bizTypePrefix))
                 .ToListAsync<SettingInfo>();
    }

    internal static Task<SettingInfo> GetUserSettingAsync(this Database db, string bizType)
    {
        return db.Query<SysSetting>()
                 .Where(d => d.CreateBy == db.UserName && d.BizType == bizType)
                 .FirstAsync<SettingInfo>();
    }

    internal static async Task<T> GetUserSettingAsync<T>(this Database db, string bizType)
    {
        var setting = await db.GetUserSettingAsync(bizType);
        if (setting == null)
            return default;

        return setting.DataAs<T>();
    }

    internal static async Task<Dictionary<string, List<TableSettingInfo>>> GetUserTableSettingsAsync(this Database db)
    {
        var settings = await db.GetUserSettingsAsync("UserTable_");
        if (settings == null || settings.Count == 0)
            return [];

        return settings.ToDictionary(k => k.BizType, v => v.DataAs<List<TableSettingInfo>>());
    }
}