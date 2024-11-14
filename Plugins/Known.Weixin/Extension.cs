using Known.Weixin.Pages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

namespace Known.Weixin;

/// <summary>
/// 依赖注入扩展类。
/// </summary>
public static class Extension
{
    private static readonly WeixinOption option = new();

    /// <summary>
    /// 添加Known框架简易微信功能模块。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">配置选项委托。</param>
    public static void AddKnownWeixin(this IServiceCollection services, Action<WeixinOption> action = null)
    {
        action?.Invoke(option);
        WeixinApi.GZHId = option.ConfigInfo?.GZHId;
        WeixinApi.AppId = option.ConfigInfo?.AppId;
        WeixinApi.AppSecret = option.ConfigInfo?.AppSecret;
        WeixinApi.RedirectUri = option.ConfigInfo?.RedirectUri;

        var assembly = typeof(Extension).Assembly;
        services.AddScoped<IWeixinService, WeixinService>();
        Config.AddModule(assembly);
        WeixinApi.Initialize();

        // 配置UI
        UIConfig.SystemTabs["WeChatSetting"] = b => b.Component<WeChatSetting>().Build();
    }

    /// <summary>
    /// 使用微信框架订阅和取消订阅路由。
    /// </summary>
    /// <param name="app">Web应用程序。</param>
    public static void UseKnownWeixin(this WebApplication app)
    {
        app.MapGet("/weixin/auth", OnAuthorizeAsync);
        app.MapGet("/weixin/subs", OnSubscribeAsync);
        app.MapGet("/weixin/unsubs", OnUnsubscribeAsync);
    }

    private static async Task OnAuthorizeAsync([FromQuery] string token, [FromQuery] string code)
    {
        await WeixinHelper.AuthorizeAsync(token, code);
        //var message = result.IsValid
        //            ? Language.Success(Language.Authorize)
        //            : Language.Failed(Language.Authorize);
        //Logger.Info(result.Message);
        //UI.Language = Language;
        //UI.Alert(message, () =>
        //{
        //    Page.Navigation.NavigateTo("/", true);
        //    return Task.CompletedTask;
        //});
        //return true;
    }

    private static async Task<string> OnSubscribeAsync(
        [FromQuery] string gzhId,
        [FromQuery] string openId,
        [FromQuery] string userId)
    {
        if (gzhId != option.ConfigInfo?.GZHId)
            return string.Empty;

        return await WeixinHelper.SubscribeAsync(openId, userId);
    }

    private static async Task<string> OnUnsubscribeAsync(
        [FromQuery] string gzhId,
        [FromQuery] string openId,
        [FromQuery] string userId)
    {
        if (gzhId != option.ConfigInfo?.GZHId)
            return string.Empty;

        return await WeixinHelper.UnsubscribeAsync(openId, userId);
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