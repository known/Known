namespace Known;

/// <summary>
/// 平台操作类，提供框架数据的常用操作方法。
/// </summary>
public class Platform
{
    //System
    /// <summary>
    /// 异步获取系统信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <returns>系统信息。</returns>
    public static Task<SystemInfo> GetSystemAsync(Database db) => SystemService.GetSystemAsync(db);

    //User
    /// <summary>
    /// 异步获取用户信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="userName">用户名。</param>
    /// <returns>用户信息。</returns>
    public static Task<UserInfo> GetUserAsync(Database db, string userName) => UserHelper.GetUserByUserNameAsync(db, userName);

    /// <summary>
    /// 异步获取角色用户列表。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="roleName">角色名称。</param>
    /// <returns>用户列表。</returns>
    public static Task<List<SysUser>> GetUsersByRoleAsync(Database db, string roleName) => UserService.GetUsersByRoleAsync(db, roleName);

    /// <summary>
    /// 异步同步系统用户到框架用户表。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="user">系统用户对象。</param>
    /// <returns></returns>
    public static Task SyncUserAsync(Database db, SysUser user) => UserService.SyncUserAsync(db, user);

    //File
    /// <summary>
    /// 异步获取系统附件信息列表。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizId">附件业务数据ID。</param>
    /// <returns>系统附件信息列表。</returns>
    public static Task<List<SysFile>> GetFilesAsync(Database db, string bizId) => FileService.GetFilesAsync(db, bizId);
    
    /// <summary>
    /// 物理删除系统附件。
    /// </summary>
    /// <param name="filePaths">附件路径列表。</param>
    public static void DeleteFiles(List<string> filePaths) => filePaths.ForEach(AttachFile.DeleteFile);

    /// <summary>
    /// 异步删除系统附件表数据。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizId">附件业务数据ID。</param>
    /// <param name="oldFiles">要物理删除的附件路径列表。</param>
    /// <returns></returns>
    public static Task DeleteFilesAsync(Database db, string bizId, List<string> oldFiles) => FileService.DeleteFilesAsync(db, bizId, oldFiles);

    /// <summary>
    /// 异步添加系统附件信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="files">表单附件列表。</param>
    /// <param name="bizId">附件业务数据ID。</param>
    /// <param name="bizType">附件业务类型。</param>
    /// <returns>系统附件信息列表。</returns>
    public static Task<List<SysFile>> AddFilesAsync(Database db, List<AttachFile> files, string bizId, string bizType) => FileService.AddFilesAsync(db, files, bizId, bizType);

    //Flow
    /// <summary>
    /// 异步创建系统工作流。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">业务工作流信息。</param>
    /// <returns></returns>
    public static Task CreateFlowAsync(Database db, FlowBizInfo info) => FlowService.CreateFlowAsync(db, info);

    /// <summary>
    /// 异步删除工作流及其日志。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizId">工作流业务数据ID。</param>
    /// <returns></returns>
    public static Task DeleteFlowAsync(Database db, string bizId) => FlowService.DeleteFlowAsync(db, bizId);

    /// <summary>
    /// 异步添加工作流日志信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizId">工作流业务数据ID。</param>
    /// <param name="stepName">流程步骤名称。</param>
    /// <param name="result">流程操作结果。</param>
    /// <param name="note">操作备注。</param>
    /// <param name="time">操作时间。</param>
    /// <returns></returns>
    public static Task AddFlowLogAsync(Database db, string bizId, string stepName, string result, string note, DateTime? time = null) => FlowService.AddFlowLogAsync(db, bizId, stepName, result, note, time);

    //Weixin
    /// <summary>
    /// 异步获取系统用户绑定的微信信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="user">系统用户。</param>
    /// <returns>微信信息。</returns>
    public static Task<SysWeixin> GetWeixinAsync(Database db, SysUser user) => WeixinService.GetWeixinByUserIdAsync(db, user.Id);
    
    /// <summary>
    /// 异步发送微信模板消息。
    /// </summary>
    /// <param name="info">模板消息信息。</param>
    /// <returns>发送结果。</returns>
    public static Task<Result> SendTemplateMessageAsync(TemplateInfo info) => WeixinApi.SendTemplateMessageAsync(info);
}