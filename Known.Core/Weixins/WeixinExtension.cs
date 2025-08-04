namespace Known.Weixins;

/// <summary>
/// 微信数据扩展类。
/// </summary>
public static class WeixinExtension
{
    /// <summary>
    /// 异步获取系统用户绑定的微信信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="userId">系统用户ID。</param>
    /// <returns>微信信息。</returns>
    public static Task<WeixinUserInfo> GetWeixinUserAsync(this Database db, string userId)
    {
        return db.Query<SysWeixin>().Where(d => d.UserId == userId).FirstAsync<WeixinUserInfo>();
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
        TaskHelper.NotifyRun(task);
        return Result.Success("Task saved！");
    }
}