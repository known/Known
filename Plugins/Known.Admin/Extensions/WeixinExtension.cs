namespace Known.Extensions;

/// <summary>
/// 微信数据扩展类。
/// </summary>
public static class WeixinExtension
{
    /// <summary>
    /// 异步获取系统用户绑定的微信信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="user">系统用户。</param>
    /// <returns>微信信息。</returns>
    public static Task<SysWeixin> GetWeixinAsync(this Database db, UserInfo user)
    {
        return WeixinService.GetWeixinByUserIdAsync(db, user.Id);
    }

    /// <summary>
    /// 异步发送微信模板消息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">模板业务信息。</param>
    /// <returns>发送结果。</returns>
    public static async Task<Result> SendTemplateMessageAsync(this Database db, WeixinTemplateInfo info)
    {
        var task = WeixinHelper.CreateTask(info);
        await db.CreateTaskAsync(task);
        TaskHelper.NotifyRun(task.Type);
        return Result.Success("Task saved！");
    }
}