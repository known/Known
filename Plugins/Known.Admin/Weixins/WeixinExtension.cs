namespace Known.Weixins;

/// <summary>
/// 依赖注入扩展类。
/// </summary>
public static class WeixinExtension
{
    private static readonly WeixinOption option = new();

    /// <summary>
    /// 添加Known框架简易微信功能模块前端。
    /// </summary>
    /// <param name="services">服务集合。</param>
    public static void AddKnownWeixin(this IServiceCollection services)
    {
        services.AddScoped<IWeixinService, WeixinService>();

        // 配置UI
        UIConfig.SystemTabs["WeChatSetting"] = b => b.Component<WeChatSetting>().Build();
    }

    /// <summary>
    /// 添加Known框架简易微信功能模块后端。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">配置选项委托。</param>
    public static void AddKnownWeixinCore(this IServiceCollection services, Action<WeixinOption> action = null)
    {
        action?.Invoke(option);
        WeixinApi.Initialize(option.ConfigInfo);
    }

    private static async Task SetUserWeixinAsync(this UserInfo user, Database db)
    {
        var weixin = await WeixinService.GetWeixinByUserIdAsync(db, user.Id);
        if (weixin == null)
            return;

        user.OpenId = weixin.OpenId;
        if (!string.IsNullOrWhiteSpace(weixin.HeadImgUrl))
            user.AvatarUrl = weixin.HeadImgUrl;
    }
}