namespace Known.Core.Extensions;

static class PlatformExtension
{
    internal static async Task InitializeTableAsync(this IPlatformService platform, Database db)
    {
        db.EnableLog = false;
        var exists = await platform.ExistsModuleAsync(db);
        if (!exists)
        {
            Console.WriteLine("Table is initializing...");
            var name = db.DatabaseType.ToString();
            foreach (var item in CoreOption.Assemblies)
            {
                var script = Utils.GetResource(item, $"{name}.sql");
                if (string.IsNullOrWhiteSpace(script))
                    continue;

                await db.ExecuteAsync(script);
            }
            Console.WriteLine("Table is initialized.");
        }
    }

    internal static async Task<string> GetSystemNameAsync(this IPlatformService platform, Database db)
    {
        var sys = await platform.GetSystemAsync(db);
        var appName = sys?.AppName;
        if (string.IsNullOrWhiteSpace(appName))
            appName = Config.App.Name;
        return appName;
    }

    internal static async Task<Result> CheckKeyAsync(this IPlatformService platform, Database db)
    {
        var info = await platform.GetSystemAsync(db);
        return Config.App.CheckSystemInfo(info);
    }

    internal static async Task<T> GetUserSettingAsync<T>(this IPlatformService platform, Database db, string bizType)
    {
        var setting = await platform.GetUserSettingAsync(db, bizType);
        if (setting == null)
            return default;

        return setting.DataAs<T>();
    }

    internal static async Task<Dictionary<string, List<TableSettingInfo>>> GetUserTableSettingsAsync(this IPlatformService platform, Database db)
    {
        var settings = await platform.GetUserSettingsAsync(db, "UserTable_");
        if (settings == null || settings.Count == 0)
            return [];

        return settings.ToDictionary(k => k.BizType, v => v.DataAs<List<TableSettingInfo>>());
    }
}