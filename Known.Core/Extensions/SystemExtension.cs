namespace Known.Extensions;

static class SystemExtension
{
    internal static async Task<SystemInfo> GetSystemAsync(this Database db)
    {
        try
        {
            var json = await db.GetConfigAsync(Constant.KeySystem);
            var info = Utils.FromJson<SystemInfo>(json);
            if (info != null)
                CoreOption.Instance.CheckSystemInfo(info);
            return info;
        }
        catch
        {
            return null;//系统未安装，返回null
        }
    }

    internal static async Task<SystemSafeInfo> GetSystemSafeAsync(this Database db)
    {
        var json = await db.GetConfigAsync(Constant.KeySystemSafe);
        return Utils.FromJson<SystemSafeInfo>(json);
    }

    internal static async Task<SystemInfo> GetUserSystemAsync(this Database db)
    {
        if (!Config.App.IsPlatform)
            return Config.System;

        var data = await db.GetCompanyDataAsync(db.User.CompNo);
        if (!string.IsNullOrWhiteSpace(data))
            return Utils.FromJson<SystemInfo>(data);

        return new SystemInfo
        {
            CompNo = db.User.CompNo,
            CompName = db.User.CompName,
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

    internal static Task<Result> SaveSystemAsync(this Database db, SystemInfo info)
    {
        return db.SaveConfigAsync(Constant.KeySystem, info);
    }
}