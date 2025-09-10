namespace Known.Extensions;

/// <summary>
/// 系统数据扩展类。
/// </summary>
public static class SystemExtension
{
    /// <summary>
    /// 异步获取系统信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <returns></returns>
    public static async Task<SystemInfo> GetSystemAsync(this Database db)
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

    internal static async Task<SystemInfo> GetUserSystemAsync(this Database db)
    {
        if (!Config.App.IsPlatform)
            return CoreConfig.System;

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