namespace Known.Core.Extensions;

static class PlatformExtension
{
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
}