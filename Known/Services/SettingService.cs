namespace Known.Services;

/// <summary>
/// 系统设置服务接口。
/// </summary>
public interface ISettingService : IService
{
    /// <summary>
    /// 异步获取用户设置信息JSON。
    /// </summary>
    /// <param name="bizType">设置业务类型。</param>
    /// <returns>用户设置信息JSON。</returns>
    Task<string> GetUserSettingAsync(string bizType);

    /// <summary>
    /// 异步删除用户设置信息。
    /// </summary>
    /// <param name="bizType">设置业务类型。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteUserSettingAsync(string bizType);

    /// <summary>
    /// 异步保存用户设置信息。
    /// </summary>
    /// <param name="info">用户设置信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveUserSettingAsync(SettingInfo info);

    /// <summary>
    /// 异步保存用户设置信息。
    /// </summary>
    /// <param name="info">设置表单信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveUserSettingAsync(SettingFormInfo info);
}

class SettingService(Context context) : ServiceBase(context), ISettingService
{
    public async Task<string> GetUserSettingAsync(string bizType)
    {
        var setting = await GetUserSettingAsync(Database, bizType);
        if (setting == null)
            return default;

        return setting.BizData;
    }

    public async Task<Result> DeleteUserSettingAsync(string bizType)
    {
        var setting = await GetUserSettingAsync(Database, bizType);
        if (setting != null)
            await Database.DeleteAsync(setting);
        return Result.Success(Language.Success(Language.Delete));
    }

    public Task<Result> SaveUserSettingAsync(SettingInfo info)
    {
        return SaveUserSettingAsync(new SettingFormInfo
        {
            BizType = SettingInfo.KeyInfo,
            BizData = info
        });
    }

    public async Task<Result> SaveUserSettingAsync(SettingFormInfo info)
    {
        var setting = await GetUserSettingAsync(Database, info.BizType);
        setting ??= new SysSetting();
        setting.BizType = info.BizType;
        setting.BizData = Utils.ToJson(info.BizData);
        await Database.SaveAsync(setting);
        return Result.Success(Language.Success(Language.Save));
    }

    internal static async Task<T> GetUserSettingAsync<T>(Database db, string bizType)
    {
        var setting = await GetUserSettingAsync(db, bizType);
        if (setting == null)
            return default;

        return setting.DataAs<T>();
    }

    private static Task<SysSetting> GetUserSettingAsync(Database db, string bizType)
    {
        return db.QueryAsync<SysSetting>(d => d.CreateBy == db.User.UserName && d.BizType == bizType);
    }
}