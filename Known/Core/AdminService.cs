namespace Known.Core;

/// <summary>
/// 管理平台操作接口，提供框架数据的常用操作方法。
/// </summary>
public interface IAdminService
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

    #region User
    /// <summary>
    /// 异步获取用户信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="userName">用户名。</param>
    /// <returns>用户信息。</returns>
    Task<UserInfo> GetUserAsync(Database db, string userName);

    /// <summary>
    /// 异步根据ID获取用户信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="userId">用户ID。</param>
    /// <returns>用户信息。</returns>
    Task<UserInfo> GetUserByIdAsync(Database db, string userId);
    #endregion

    #region Setting
    /// <summary>
    /// 异步获取用户设置信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizType">业务类型。</param>
    /// <returns>设置信息。</returns>
    Task<SettingInfo> GetUserSettingAsync(Database db, string bizType);

    /// <summary>
    /// 异步保存设置信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">设置信息。</param>
    /// <returns></returns>
    Task SaveSettingAsync(Database db, SettingInfo info);
    #endregion

    #region File
    /// <summary>
    /// 异步获取系统附件信息列表。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizId">附件业务数据ID。</param>
    /// <returns>系统附件信息列表。</returns>
    Task<List<AttachInfo>> GetFilesAsync(Database db, string bizId);

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
    /// 异步删除系统附件实体。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="id">附件ID。</param>
    /// <returns></returns>
    Task DeleteFileAsync(Database db, string id);

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
    /// 异步获取指定业务的系统后台任务信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizId">业务ID。</param>
    /// <returns>系统后台任务信息。</returns>
    Task<TaskInfo> GetTaskAsync(Database db, string bizId);

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