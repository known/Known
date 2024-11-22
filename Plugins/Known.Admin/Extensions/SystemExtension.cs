namespace Known.Extensions;

static class SystemExtension
{
    internal static async Task<SystemInfo> GetSystemAsync(this Database db)
    {
        if (!Config.App.IsPlatform || db.User == null)
        {
            var json = await db.GetConfigAsync(Constant.KeySystem);
            return Utils.FromJson<SystemInfo>(json);
        }

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

    internal static async Task<string> GetSystemNameAsync(this Database db)
    {
        var sys = await db.GetSystemAsync();
        var appName = sys?.AppName;
        if (string.IsNullOrWhiteSpace(appName))
            appName = Config.App.Name;
        return appName;
    }

    internal static Task<Result> SaveSystemAsync(this Database db, SystemInfo info)
    {
        return db.SaveConfigAsync(Constant.KeySystem, info);
    }

    internal static async Task<Result> CheckKeyAsync(this Database db)
    {
        var info = await db.GetSystemAsync();
        return AdminOption.Instance.CheckSystemInfo(info);
    }
}