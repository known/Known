namespace Known.Services;

/// <summary>
/// 框架平台数据服务接口。
/// </summary>
public interface IPlatformService : IService
{
    #region Config
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
    #endregion

    #region User
    /// <summary>
    /// 异步获取用户信息。
    /// </summary>
    /// <param name="userName">用户名。</param>
    /// <returns>用户信息。</returns>
    Task<UserInfo> GetUserAsync(string userName);

    /// <summary>
    /// 异步根据ID获取用户信息。
    /// </summary>
    /// <param name="userId">用户ID。</param>
    /// <returns>用户信息。</returns>
    Task<UserInfo> GetUserByIdAsync(string userId);
    #endregion

    #region Setting
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
    Task<Result> SaveUserSettingFormAsync(SettingFormInfo info);
    #endregion

    #region File
    /// <summary>
    /// 异步获取系统附件列表。
    /// </summary>
    /// <param name="bizId">附件业务数据ID。</param>
    /// <returns>系统附件列表。</returns>
    Task<List<AttachInfo>> GetFilesAsync(string bizId);

    /// <summary>
    /// 异步删除单条系统附件。
    /// </summary>
    /// <param name="file">系统附件对象。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteFileAsync(AttachInfo file);
    #endregion

    #region Log
    /// <summary>
    /// 异步添加系统日志。
    /// </summary>
    /// <param name="log">系统日志</param>
    /// <returns>添加结果。</returns>
    Task<Result> AddLogAsync(LogInfo log);
    #endregion
}

class PlatformService(HttpClient http) : ClientBase(http), IPlatformService
{
    public Task<string> GetConfigAsync(string key)
    {
        return Http.GetStringAsync($"/Platform/GetConfig?key={key}");
    }

    public Task<Result> SaveConfigAsync(ConfigInfo info)
    {
        return Http.PostAsync("/Platform/SaveConfig", info);
    }

    public Task<UserInfo> GetUserAsync(string userName)
    {
        return Http.GetAsync<UserInfo>($"/Platform/GetUser?userName={userName}");
    }

    public Task<UserInfo> GetUserByIdAsync(string userId)
    {
        return Http.GetAsync<UserInfo>($"/Platform/GetUserById?userId={userId}");
    }

    public Task<string> GetUserSettingAsync(string bizType)
    {
        return Http.GetStringAsync($"/Platform/GetUserSetting?bizType={bizType}");
    }

    public Task<Result> SaveUserSettingFormAsync(SettingFormInfo info)
    {
        return Http.PostAsync("/Platform/SaveConfig", info);
    }

    public Task<List<AttachInfo>> GetFilesAsync(string bizId)
    {
        return Http.GetAsync<List<AttachInfo>>($"/Platform/GetFiles?bizId={bizId}");
    }

    public Task<Result> DeleteFileAsync(AttachInfo file)
    {
        return Http.PostAsync("/Platform/DeleteFile", file);
    }

    public Task<Result> AddLogAsync(LogInfo log)
    {
        return Http.PostAsync("/Platform/AddLog", log);
    }
}