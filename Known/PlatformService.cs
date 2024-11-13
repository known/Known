namespace Known;

/// <summary>
/// 平台操作接口，提供框架数据的常用操作方法。
/// </summary>
public interface IPlatformService : IService
{
    #region Config
    /// <summary>
    /// 异步获取配置信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="key">配置键。</param>
    /// <returns>配置数据JSON字符串。</returns>
    Task<string> GetConfigAsync(Database db, string key);

    /// <summary>
    /// 异步保存配置信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="key">配置键。</param>
    /// <param name="value">配置对象。</param>
    /// <returns></returns>
    Task SaveConfigAsync(Database db, string key, object value);
    #endregion

    #region Company
    /// <summary>
    /// 异步获取租户企业信息。
    /// </summary>
    /// <returns>企业信息JSO你。</returns>
    Task<string> GetCompanyAsync();

    /// <summary>
    /// 异步保存企业信息。
    /// </summary>
    /// <param name="model">企业信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveCompanyAsync(object model);
    #endregion

    #region User
    /// <summary>
    /// 异步分页查询系统用户。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<UserInfo>> QueryUsersAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步获取用户信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="userName">用户名。</param>
    /// <returns>用户信息。</returns>
    Task<UserInfo> GetUserAsync(Database db, string userName);
    #endregion

    #region Import
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

    #region File
    /// <summary>
    /// 异步获取系统附件列表。
    /// </summary>
    /// <param name="bizId">附件业务数据ID。</param>
    /// <returns>系统附件列表。</returns>
    Task<List<AttachInfo>> GetFilesAsync(string bizId);

    /// <summary>
    /// 异步添加系统附件信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="files">表单附件列表。</param>
    /// <param name="bizId">附件业务数据ID。</param>
    /// <param name="bizType">附件业务类型。</param>
    /// <returns>系统附件信息列表。</returns>
    Task<List<AttachInfo>> AddFilesAsync(Database db, List<AttachFile> files, string bizId, string bizType);

    /// <summary>
    /// 异步删除单条系统附件。
    /// </summary>
    /// <param name="file">系统附件对象。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteFileAsync(AttachInfo file);

    /// <summary>
    /// 异步删除系统附件表数据。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizId">附件业务数据ID。</param>
    /// <param name="oldFiles">要物理删除的附件路径列表。</param>
    /// <returns></returns>
    Task DeleteFilesAsync(Database db, string bizId, List<string> oldFiles);
    #endregion

    #region Task
    /// <summary>
    /// 异步创建一个系统后台任务。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">任务信息。</param>
    /// <returns></returns>
    Task CreateTaskAsync(Database db, TaskInfo info);
    #endregion

    #region Log
    /// <summary>
    /// 异步添加系统日志。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="log">系统日志</param>
    /// <returns>添加结果。</returns>
    Task<Result> AddLogAsync(Database db, LogInfo log);
    #endregion
}