namespace Known.Services;

/// <summary>
/// 框架数据服务接口。
/// </summary>
public interface IDataService : IService
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
    Task SaveConfigAsync(ConfigInfo info);

    /// <summary>
    /// 异步添加系统日志。
    /// </summary>
    /// <param name="log">系统日志</param>
    /// <returns>添加结果。</returns>
    Task<Result> AddLogAsync(LogInfo log);

    #region Setting
    /// <summary>
    /// 异步获取用户设置信息JSON。
    /// </summary>
    /// <param name="bizType">设置业务类型。</param>
    /// <returns>用户设置信息JSON。</returns>
    Task<string> GetUserSettingAsync(string bizType);

    /// <summary>
    /// 异步保存用户系统设置信息。
    /// </summary>
    /// <param name="info">用户设置信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveUserSettingAsync(UserSettingInfo info);

    /// <summary>
    /// 异步保存用户业务设置信息，如：高级查询。
    /// </summary>
    /// <param name="info">设置表单信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveUserSettingFormAsync(SettingFormInfo info);
    #endregion

    #region Import
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

    /// <summary>
    /// 异步获取导入表单数据信息。
    /// </summary>
    /// <param name="bizId">业务数据ID。</param>
    /// <returns>导入表单数据信息。</returns>
    Task<ImportFormInfo> GetImportAsync(string bizId);

    /// <summary>
    /// 异步获取数据导入规范文件。
    /// </summary>
    /// <param name="bizId">业务数据ID。</param>
    /// <returns>导入规范文件。</returns>
    Task<byte[]> GetImportRuleAsync(string bizId);

    /// <summary>
    /// 异步导入系统附件。
    /// </summary>
    /// <param name="info">系统附件信息。</param>
    /// <returns>导入结果。</returns>
    Task<Result> ImportFilesAsync(UploadInfo<ImportFormInfo> info);
    #endregion
}

class DataService(HttpClient http) : ClientBase(http), IDataService
{
    public Task<string> GetConfigAsync(string key)
    {
        throw new NotImplementedException();
    }

    public Task SaveConfigAsync(ConfigInfo info)
    {
        throw new NotImplementedException();
    }

    public Task<Result> AddLogAsync(LogInfo log)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetUserSettingAsync(string bizType)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SaveUserSettingAsync(UserSettingInfo info)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SaveUserSettingFormAsync(SettingFormInfo info)
    {
        throw new NotImplementedException();
    }

    public Task<List<AttachInfo>> GetFilesAsync(string bizId)
    {
        throw new NotImplementedException();
    }

    public Task<Result> DeleteFileAsync(AttachInfo file)
    {
        throw new NotImplementedException();
    }

    public Task<ImportFormInfo> GetImportAsync(string bizId)
    {
        throw new NotImplementedException();
    }

    public Task<byte[]> GetImportRuleAsync(string bizId)
    {
        throw new NotImplementedException();
    }

    public Task<Result> ImportFilesAsync(UploadInfo<ImportFormInfo> info)
    {
        throw new NotImplementedException();
    }
}