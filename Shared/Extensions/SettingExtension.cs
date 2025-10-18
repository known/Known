namespace Known.Extensions;

/// <summary>
/// 用户设置数据扩展类。
/// </summary>
public static class SettingExtension
{
    internal static async Task<bool> CheckUserDefaultPasswordAsync(this Database db, SystemInfo info)
    {
        var user = await db.QueryAsync<SysUser>(d => d.UserName == db.UserName);
        var password = Utils.ToMd5(info.UserDefaultPwd);
        return user != null && user.Password == password;
    }

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

    /// <summary>
    /// 异步删除用户设置信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizType">设置业务类型。</param>
    /// <returns></returns>
    public static async Task DeleteUserSettingAsync(this Database db, string bizType)
    {
        var userName = db.UserName;
        await db.DeleteAsync<SysSetting>(d => d.CreateBy == userName && d.BizType == bizType);
    }

    /// <summary>
    /// 异步保存用户设置信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizType">设置业务类型。</param>
    /// <param name="bizData">设置业务数据。</param>
    /// <returns></returns>
    public static async Task SaveUserSettingAsync(this Database db, string bizType, object bizData)
    {
        var setting = await db.GetUserSettingAsync(bizType);
        if (setting != null && bizData == null)
        {
            await db.DeleteAsync<SysSetting>(setting.Id);
        }
        else
        {
            setting ??= new SettingInfo();
            setting.BizType = bizType;
            setting.BizData = Utils.ToJson(bizData);
            await db.SaveSettingAsync(setting);
        }
    }

    private static async Task SaveSettingAsync(this Database db, SettingInfo info)
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