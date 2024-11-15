namespace Known.Extensions;

/// <summary>
/// 平台服务扩展类。
/// </summary>
public static class AdminExtension
{
    #region User
    /// <summary>
    /// 异步获取角色用户列表。
    /// </summary>
    /// <param name="service">平台服务实例。</param>
    /// <param name="db">数据库对象。</param>
    /// <param name="roleName">角色名称。</param>
    /// <returns>用户列表。</returns>
    public static Task<List<UserInfo>> GetUsersByRoleAsync(this IAdminService service, Database db, string roleName)
    {
        return Platform.GetUsersByRoleAsync(db, roleName);
    }

    /// <summary>
    /// 异步同步系统用户到框架用户表。
    /// </summary>
    /// <param name="service">平台服务实例。</param>
    /// <param name="db">数据库对象。</param>
    /// <param name="user">系统用户对象。</param>
    /// <returns></returns>
    public static Task SyncUserAsync(this IAdminService service, Database db, SysUser user)
    {
        return Platform.SyncUserAsync(db, user);
    }
    #endregion

    #region Log
    /// <summary>
    /// 异步获取常用功能菜单信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="userName">用户名。</param>
    /// <param name="size">Top数量。</param>
    /// <returns>功能菜单信息。</returns>
    public static async Task<List<string>> GetVisitMenuIdsAsync(this Database db, string userName, int size)
    {
        var logs = await db.Query<SysLog>()
                           .Where(d => d.Type == $"{LogType.Page}" && d.CreateBy == userName)
                           .GroupBy(d => d.Target)
                           .Select(d => new CountInfo { Field1 = d.Target, TotalCount = DbFunc.Count() })
                           .ToListAsync();
        logs = logs?.OrderByDescending(f => f.TotalCount).Take(size).ToList();
        return logs?.Select(l => l.Field1).ToList();
    }
    #endregion

    #region Weixin
    /// <summary>
    /// 异步获取系统用户绑定的微信信息。
    /// </summary>
    /// <param name="service">平台服务实例。</param>
    /// <param name="db">数据库对象。</param>
    /// <param name="user">系统用户。</param>
    /// <returns>微信信息。</returns>
    public static Task<SysWeixin> GetWeixinAsync(this IAdminService service, Database db, UserInfo user)
    {
        return WeixinService.GetWeixinByUserIdAsync(db, user.Id);
    }

    /// <summary>
    /// 异步发送微信模板消息。
    /// </summary>
    /// <param name="service">平台服务实例。</param>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">模板业务信息。</param>
    /// <returns>发送结果。</returns>
    public static async Task<Result> SendTemplateMessageAsync(this IAdminService service, Database db, WeixinTemplateInfo info)
    {
        var task = WeixinHelper.CreateTask(info);
        await service.CreateTaskAsync(db, task);
        TaskHelper.NotifyRun(task.Type);
        return Result.Success("Task saved！");
    }
    #endregion

    #region WorkFlow
    /// <summary>
    /// 异步创建系统工作流。
    /// </summary>
    /// <param name="service">平台服务实例。</param>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">业务工作流信息。</param>
    /// <returns></returns>
    public static Task CreateFlowAsync(this IAdminService service, Database db, FlowBizInfo info)
    {
        return FlowService.CreateFlowAsync(db, info);
    }

    /// <summary>
    /// 异步删除工作流及其日志。
    /// </summary>
    /// <param name="service">平台服务实例。</param>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizId">工作流业务数据ID。</param>
    /// <returns></returns>
    public static Task DeleteFlowAsync(this IAdminService service, Database db, string bizId)
    {
        return FlowService.DeleteFlowAsync(db, bizId);
    }

    /// <summary>
    /// 异步添加工作流日志信息。
    /// </summary>
    /// <param name="service">平台服务实例。</param>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizId">工作流业务数据ID。</param>
    /// <param name="stepName">流程步骤名称。</param>
    /// <param name="result">流程操作结果。</param>
    /// <param name="note">操作备注。</param>
    /// <param name="time">操作时间。</param>
    /// <returns></returns>
    public static Task AddFlowLogAsync(this IAdminService service, Database db, string bizId, string stepName, string result, string note, DateTime? time = null)
    {
        return FlowService.AddFlowLogAsync(db, bizId, stepName, result, note, time);
    }
    #endregion
}