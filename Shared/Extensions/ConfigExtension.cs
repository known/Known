namespace Known.Extensions;

/// <summary>
/// 配置数据扩展类。
/// </summary>
public static class ConfigExtension
{
    /// <summary>
    /// 异步判断配置数据是否存在。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="key">配置键。</param>
    /// <returns>是否存在。</returns>
    public static Task<bool> ExistsConfigAsync(this Database db, string key)
    {
        var appId = Config.App.Id;
        return db.ExistsAsync<SysConfig>(d => d.AppId == appId && d.ConfigKey == key);
    }

    /// <summary>
    /// 异步获取配置数据字符串。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="key">配置键。</param>
    /// <returns>配置数据字符串。</returns>
    public static async Task<string> GetConfigAsync(this Database db, string key)
    {
        var appId = Config.App.Id;
        var config = await db.QueryAsync<SysConfig>(d => d.AppId == appId && d.ConfigKey == key);
        return config?.ConfigValue;
    }

    /// <summary>
    /// 异步保存字符串配置数据。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="key">配置键。</param>
    /// <param name="value">配置字符串。</param>
    /// <returns>保存结果。</returns>
    public static async Task<Result> SaveConfigAsync(this Database db, string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            return Result.Error(Language.TipKeyRequired);

        var appId = Config.App.Id;
        var data = new Dictionary<string, object>
        {
            [nameof(SysConfig.AppId)] = appId,
            [nameof(SysConfig.ConfigKey)] = key,
            [nameof(SysConfig.ConfigValue)] = value
        };
        if (await db.ExistsConfigAsync(key))
            await db.UpdateAsync(nameof(SysConfig), "AppId,ConfigKey", data);
        else
            await db.InsertAsync(nameof(SysConfig), data);
        return Result.Success(Language.SaveSuccess);
    }

    /// <summary>
    /// 异步获取系统信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="isCheck">是否检查系统。</param>
    /// <returns></returns>
    public static async Task<SystemInfo> GetSystemAsync(this Database db, bool isCheck = false)
    {
        try
        {
            var isExist = await db.ExistsAsync<SysConfig>();
            if (!isExist)
                return null;

            var json = await db.GetConfigAsync(Constants.KeySystem);
            var info = Utils.FromJson<SystemInfo>(json);
            if (info != null && isCheck)
                CoreConfig.CheckSystemInfo(info);
            return info;
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
            return null;//系统未安装，返回null
        }
    }

    /// <summary>
    /// 异步保存系统信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">系统信息。</param>
    /// <returns></returns>
    public static Task<Result> SaveSystemAsync(this Database db, SystemInfo info)
    {
        return db.SaveConfigAsync(Constants.KeySystem, info);
    }

    /// <summary>
    /// 异步获取配置数据。
    /// </summary>
    /// <typeparam name="T">数据类型。</typeparam>
    /// <param name="db">数据库对象。</param>
    /// <param name="key">配置键。</param>
    /// <param name="isGZip">是否压缩数据。</param>
    /// <returns>配置对象。</returns>
    public static async Task<T> GetConfigAsync<T>(this Database db, string key, bool isGZip = false)
    {
        var value = await db.GetConfigAsync(key);
        if (!isGZip)
            return Utils.FromJson<T>(value);

        return ZipHelper.UnZipDataAsString<T>(value);
    }

    /// <summary>
    /// 异步保存配置数据。
    /// </summary>
    /// <typeparam name="T">数据类型。</typeparam>
    /// <param name="db">数据库对象。</param>
    /// <param name="key">配置键。</param>
    /// <param name="value">配置对象。</param>
    /// <param name="isGZip">是否压缩数据。</param>
    /// <returns>保存结果。</returns>
    public static Task<Result> SaveConfigAsync<T>(this Database db, string key, T value, bool isGZip = false)
    {
        var text = isGZip ? ZipHelper.ZipDataAsString(value) : Utils.ToJson(value);
        return db.SaveConfigAsync(key, text);
    }
}