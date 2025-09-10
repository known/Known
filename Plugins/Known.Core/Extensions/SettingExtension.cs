namespace Known.Extensions;

/// <summary>
/// 用户设置数据扩展类。
/// </summary>
public static class SettingExtension
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

    /// <summary>
    /// 异步获取用户设置信息。
    /// </summary>
    /// <typeparam name="T">设置信息类型。</typeparam>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizType">设置业务类型。</param>
    /// <returns></returns>
    public static async Task<T> GetUserSettingAsync<T>(this Database db, string bizType)
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

        var dics = new Dictionary<string, List<TableSettingInfo>>();
        foreach (var item in settings)
        {
            dics[item.BizType] = item.DataAs<List<TableSettingInfo>>();
        }
        return dics;
    }

    /// <summary>
    /// 异步保存设置信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">设置信息。</param>
    /// <returns></returns>
    public static async Task SaveSettingAsync(this Database db, SettingInfo info)
    {
        var model = await db.QueryByIdAsync<SysSetting>(info.Id);
        model ??= new SysSetting();
        if (!string.IsNullOrWhiteSpace(info.Id))
            model.Id = info.Id;
        model.BizType = info.BizType;
        model.BizData = info.BizData;
        await db.SaveAsync(model);
    }
}