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
    /// 异步保存用户系统设置信息。
    /// </summary>
    /// <param name="info">用户设置信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveUserSettingInfoAsync(SettingInfo info);

    /// <summary>
    /// 异步保存用户业务设置信息，如：高级查询。
    /// </summary>
    /// <param name="info">设置表单信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveUserSettingFormAsync(SettingFormInfo info);
}