namespace Known.Extensions;

/// <summary>
/// 平台服务扩展类。
/// </summary>
public static class PlatformExtension
{
    #region Config
    /// <summary>
    /// 异步获取指定类型的配置信息。
    /// </summary>
    /// <typeparam name="T">配置对象类型。</typeparam>
    /// <param name="platform">平台服务实例。</param>
    /// <param name="db">数据库对象。</param>
    /// <param name="key">配置键。</param>
    /// <returns>配置对象。</returns>
    public static async Task<T> GetConfigAsync<T>(this IPlatformService platform, Database db, string key)
    {
        var json = await platform.GetConfigAsync(db, key);
        return Utils.FromJson<T>(json);
    }
    #endregion

    #region System
    private const string KeySystem = "SystemInfo";

    /// <summary>
    /// 异步获取系统配置信息，如果是平台，则获取租户配置信息。
    /// </summary>
    /// <param name="platform">平台服务实例。</param>
    /// <param name="db">数据库对象。</param>
    /// <returns>系统配置信息。</returns>
    public static async Task<SystemInfo> GetSystemAsync(this IPlatformService platform, Database db)
    {
        if (!Config.App.IsPlatform || db.User == null)
        {
            var json = await platform.GetConfigAsync(db, KeySystem);
            return Utils.FromJson<SystemInfo>(json);
        }

        var data = await platform.GetCompanyDataAsync(db, db.User.CompNo);
        if (!string.IsNullOrWhiteSpace(data))
            return Utils.FromJson<SystemInfo>(data);

        return new SystemInfo
        {
            CompNo = db.User.CompNo,
            CompName = db.User.CompName,
            AppName = Config.App.Name
        };
    }

    /// <summary>
    /// 异步保存系统配置信息。
    /// </summary>
    /// <param name="platform">平台服务实例。</param>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">系统配置信息。</param>
    /// <returns></returns>
    public static Task SaveSystemAsync(this IPlatformService platform, Database db, SystemInfo info)
    {
        return platform.SaveConfigAsync(db, KeySystem, info);
    }
    #endregion

    #region File
    /// <summary>
    /// 物理删除系统附件。
    /// </summary>
    /// <param name="platform">平台服务实例。</param>
    /// <param name="filePaths">附件路径列表。</param>
    public static void DeleteFiles(this IPlatformService platform, List<string> filePaths)
    {
        filePaths.ForEach(AttachFile.DeleteFile);
    }
    #endregion
}