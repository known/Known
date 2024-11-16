namespace Known.Services;

/// <summary>
/// 系统服务接口。
/// </summary>
public interface ISystemService : IService
{
    /// <summary>
    /// 异步获取系统信息。
    /// </summary>
    /// <returns>系统信息。</returns>
    [AllowAnonymous] Task<SystemInfo> GetSystemAsync();

    /// <summary>
    /// 异步获取系统安装信息。
    /// </summary>
    /// <returns>系统安装信息。</returns>
    [AllowAnonymous] Task<InstallInfo> GetInstallAsync();

    /// <summary>
    /// 异步测试数据库连接。
    /// </summary>
    /// <param name="info">数据库连接信息。</param>
    /// <returns>测试结果。</returns>
    [AllowAnonymous] Task<Result> TestConnectionAsync(DatabaseInfo info);

    /// <summary>
    /// 异步保存系统安装信息。
    /// </summary>
    /// <param name="info">系统安装信息。</param>
    /// <returns>保存结果。</returns>
    [AllowAnonymous] Task<Result> SaveInstallAsync(InstallInfo info);

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
    /// 异步获取系统数据信息。
    /// </summary>
    /// <returns>系统数据信息。</returns>
    Task<SystemDataInfo> GetSystemDataAsync();

    /// <summary>
    /// 异步保存系统信息。
    /// </summary>
    /// <param name="info">系统信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveSystemAsync(SystemInfo info);

    /// <summary>
    /// 异步保存产品Key。
    /// </summary>
    /// <param name="info">系统信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveKeyAsync(SystemInfo info);

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
    Task<Result> SaveUserSettingInfoAsync(UserSettingInfo info);

    /// <summary>
    /// 异步保存用户业务设置信息，如：高级查询。
    /// </summary>
    /// <param name="info">设置表单信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveUserSettingFormAsync(SettingFormInfo info);
    #endregion

    #region Company
    /// <summary>
    /// 异步获取租户企业信息JSON。
    /// </summary>
    /// <returns>企业信息JSON。</returns>
    Task<string> GetCompanyAsync();

    /// <summary>
    /// 异步保存租户企业信息。
    /// </summary>
    /// <param name="model">企业信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveCompanyAsync(object model);

    /// <summary>
    /// 异步分页查询系统用户。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<UserInfo>> QueryUsersAsync(PagingCriteria criteria);
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

class SystemService(HttpClient http) : ClientBase(http), ISystemService
{
    public Task<SystemInfo> GetSystemAsync()
    {
        throw new NotImplementedException();
    }

    public Task<InstallInfo> GetInstallAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Result> TestConnectionAsync(DatabaseInfo info)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SaveInstallAsync(InstallInfo info)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetConfigAsync(string key)
    {
        throw new NotImplementedException();
    }

    public Task SaveConfigAsync(ConfigInfo info)
    {
        throw new NotImplementedException();
    }

    public Task<SystemDataInfo> GetSystemDataAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Result> SaveSystemAsync(SystemInfo info)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SaveKeyAsync(SystemInfo info)
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

    public Task<Result> DeleteUserSettingAsync(string bizType)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SaveUserSettingInfoAsync(UserSettingInfo info)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SaveUserSettingFormAsync(SettingFormInfo info)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetCompanyAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Result> SaveCompanyAsync(object model)
    {
        throw new NotImplementedException();
    }

    public Task<PagingResult<UserInfo>> QueryUsersAsync(PagingCriteria criteria)
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