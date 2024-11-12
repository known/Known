namespace Known.Weixin.Extensions;

/// <summary>
/// 微信平台服务扩展类。
/// </summary>
public static class PlatformExtension
{
    /// <summary>
    /// 异步获取系统用户绑定的微信信息。
    /// </summary>
    /// <param name="platform">平台服务实例。</param>
    /// <param name="db">数据库对象。</param>
    /// <param name="user">系统用户。</param>
    /// <returns>微信信息。</returns>
    public static Task<SysWeixin> GetWeixinAsync(this IPlatformService platform, Database db, SysUser user)
    {
        return WeixinService.GetWeixinByUserIdAsync(db, user.Id);
    }

    /// <summary>
    /// 异步发送微信模板消息。
    /// </summary>
    /// <param name="platform">平台服务实例。</param>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">模板业务信息。</param>
    /// <returns>发送结果。</returns>
    public static Task<Result> SendTemplateMessageAsync(this IPlatformService platform, Database db, WeixinTemplateInfo info)
    {
        return WeixinService.SendTemplateMessageAsync(db, info);
    }
}