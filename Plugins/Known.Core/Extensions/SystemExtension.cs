namespace Known.Extensions;

/// <summary>
/// 系统数据扩展类。
/// </summary>
public static class SystemExtension
{
    internal static Task<SystemInfo> GetUserSystemAsync(this Database db)
    {
        //if (!Config.App.IsPlatform)
        //    return CoreConfig.System;

        //var data = await db.GetCompanyDataAsync(db.User.CompNo);
        //if (!string.IsNullOrWhiteSpace(data))
        //    return Utils.FromJson<SystemInfo>(data);

        //return new SystemInfo
        //{
        //    CompNo = db.User.CompNo,
        //    CompName = db.User.CompName,
        //    AppName = Config.App.Name
        //};
        var info = !Config.App.IsPlatform
                 ? CoreConfig.System
                 : new SystemInfo { CompNo = db.User.CompNo, CompName = db.User.CompName, AppName = Config.App.Name };
        return Task.FromResult(info);
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