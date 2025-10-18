namespace Known.Services;

public partial interface IAdminService
{
    /// <summary>
    /// 异步获取系统配置数据。
    /// </summary>
    /// <param name="key">配置数据键。</param>
    /// <returns>配置数据JSON字符串。</returns>
    Task<string> GetConfigAsync(string key);

    /// <summary>
    /// 异步保存系统配置数据。
    /// </summary>
    /// <param name="info">系统配置数据信息。</param>
    /// <returns></returns>
    Task<Result> SaveConfigAsync(ConfigInfo info);

    /// <summary>
    /// 异步获取用户设置信息JSON。
    /// </summary>
    /// <param name="bizType">设置业务类型。</param>
    /// <returns>用户设置信息JSON。</returns>
    Task<string> GetUserSettingAsync(string bizType);

    /// <summary>
    /// 异步保存用户业务设置信息，如：高级查询。
    /// </summary>
    /// <param name="info">设置表单信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveUserSettingAsync(SettingFormInfo info);

    /// <summary>
    /// 异步重置用户系统设置信息。
    /// </summary>
    /// <returns>重置结果。</returns>
    Task<Result> ResetUserSettingAsync();
}

partial class AdminClient
{
    public Task<string> GetConfigAsync(string key) => Http.GetTextAsync($"/Admin/GetConfig?key={key}");
    public Task<Result> SaveConfigAsync(ConfigInfo info) => Http.PostAsync("/Admin/SaveConfig", info);
    public Task<string> GetUserSettingAsync(string bizType) => Http.GetTextAsync($"/Admin/GetUserSetting?bizType={bizType}");
    public Task<Result> SaveUserSettingAsync(SettingFormInfo info) => Http.PostAsync("/Admin/SaveUserSetting", info);
    public Task<Result> ResetUserSettingAsync() => Http.PostAsync("/Admin/ResetUserSetting");
}

partial class AdminService
{
    public Task<string> GetConfigAsync(string key)
    {
        return Database.GetConfigAsync(key);
    }

    public Task<Result> SaveConfigAsync(ConfigInfo info)
    {
        return Database.SaveConfigAsync(info.Key, info.Value);
    }

    public async Task<string> GetUserSettingAsync(string bizType)
    {
        var setting = await Database.GetUserSettingAsync(bizType);
        if (setting == null)
            return default;

        return setting.BizData;
    }

    public async Task<Result> SaveUserSettingAsync(SettingFormInfo info)
    {
        await Database.SaveUserSettingAsync(info.BizType, info.BizData);
        return Result.Success(Language.SaveSuccess);
    }

    public async Task<Result> ResetUserSettingAsync()
    {
        var result = await SaveUserSettingAsync(new SettingFormInfo
        {
            BizType = Constants.UserSetting,
            BizData = null
        });
        result.Data = CoreConfig.UserSetting.Clone();
        return result;
    }
}