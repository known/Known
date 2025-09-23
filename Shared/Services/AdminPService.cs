namespace Known.Services;

/// <summary>
/// 管理后台服务接口。
/// </summary>
public interface IAdminPService
{
    #region File
    /// <summary>
    /// 异步获取系统附件信息列表。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizIds">业务ID集合。</param>
    /// <returns></returns>
    Task<List<AttachInfo>> GetAttachesAsync(Database db, string[] bizIds);

    /// <summary>
    /// 异步获取系统附件信息列表。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizId">业务ID。</param>
    /// <returns></returns>
    Task<List<AttachInfo>> GetAttachesAsync(Database db, string bizId);

    /// <summary>
    /// 异步获取系统附件信息列表。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizId">业务ID。</param>
    /// <param name="bizType">业务类型。</param>
    /// <returns></returns>
    Task<List<AttachInfo>> GetAttachesAsync(Database db, string bizId, string bizType);

    /// <summary>
    /// 异步获取系统附件信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="id">附件ID。</param>
    /// <returns></returns>
    Task<AttachInfo> GetAttachAsync(Database db, string id);

    /// <summary>
    /// 异步删除一条系统附件数据。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="id">附件ID。</param>
    /// <returns></returns>
    Task DeleteFileAsync(Database db, string id);

    /// <summary>
    /// 异步添加附件信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">附件信息。</param>
    /// <returns></returns>
    Task<AttachInfo> AddFileAsync(Database db, AttachFile info);
    #endregion

    #region Task
    /// <summary>
    /// 异步根据业务ID获取任务信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizId">业务ID。</param>
    /// <returns></returns>
    Task<TaskInfo> GetTaskAsync(Database db, string bizId);

    /// <summary>
    /// 异步创建一个后台任务。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">任务信息。</param>
    /// <returns></returns>
    Task CreateTaskAsync(Database db, TaskInfo info);

    /// <summary>
    /// 异步保存任务信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">任务信息。</param>
    /// <returns></returns>
    Task SaveTaskAsync(Database db, TaskInfo info);
    #endregion

    #region Log
    /// <summary>
    /// 异步获取常用功能菜单信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="userName">用户名。</param>
    /// <param name="size">Top数量。</param>
    /// <returns>功能菜单信息。</returns>
    Task<List<string>> GetVisitMenuIdsAsync(Database db, string userName, int size);

    /// <summary>
    /// 异步保存日志信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">日志信息。</param>
    /// <returns></returns>
    Task SaveLogAsync(Database db, LogInfo info);
    #endregion
}