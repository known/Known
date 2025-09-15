namespace Known.Extensions;

/// <summary>
/// 系统数据扩展类。
/// </summary>
public static class SystemExtension
{
    internal static async Task<SystemInfo> GetUserSystemAsync(this Database db, UserInfo info = null)
    {
        if (!Config.App.IsPlatform)
            return Config.System;

        var user = info ?? db.User;
        var data = await db.GetCompanyDataAsync(user.CompNo);
        if (!string.IsNullOrWhiteSpace(data))
            return Utils.FromJson<SystemInfo>(data);

        return new SystemInfo
        {
            CompNo = user.CompNo,
            CompName = user.CompName,
            AppName = Config.App.Name
        };
    }

    internal static async Task<string> GetUserSystemNameAsync(this Database db)
    {
        var sys = await db.GetUserSystemAsync();
        var appName = sys?.AppName;
        if (string.IsNullOrWhiteSpace(appName))
            appName = Config.App.Name;
        return appName;
    }
}