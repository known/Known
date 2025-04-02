namespace Known.Services;

public partial interface IAdminService
{
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

partial class AdminService
{
    public Task<string> GetUserSettingAsync(string bizType)
    {
        Configs.TryGetValue(bizType, out var value);
        return Task.FromResult(value);
    }

    public Task<Result> SaveUserSettingAsync(SettingFormInfo info)
    {
        if (info.BizData != null)
            Configs[info.BizType] = Utils.ToJson(info.BizData);
        return Result.SuccessAsync(Language.SaveSuccess);
    }

    public Task<Result> ResetUserSettingAsync()
    {
        var info = new UserSettingInfo();
        return Result.SuccessAsync("重置成功！", info);
    }
}

partial class AdminClient
{
    public Task<string> GetUserSettingAsync(string bizType)
    {
        return Http.GetTextAsync($"/Admin/GetUserSetting?bizType={bizType}");
    }

    public Task<Result> SaveUserSettingAsync(SettingFormInfo info)
    {
        return Http.PostAsync("/Admin/SaveUserSetting", info);
    }

    public Task<Result> ResetUserSettingAsync()
    {
        return Http.PostAsync("/Admin/ResetUserSetting");
    }
}