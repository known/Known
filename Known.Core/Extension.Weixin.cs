namespace Known.Core;

/// <summary>
/// 微信依赖注入扩展类。
/// </summary>
public static partial class Extension
{
    /// <summary>
    /// 添加微信框架支持。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configuration">应用配置。</param>
    public static void AddKnownWeixin(this IServiceCollection services, IConfiguration configuration)
    {
        WeixinApi.GZHId = configuration.GetSection("WxGZHId").Get<string>();
        WeixinApi.AppId = configuration.GetSection("WxAppId").Get<string>();
        WeixinApi.AppSecret = configuration.GetSection("WxAppSecret").Get<string>();
        WeixinApi.RedirectUri = configuration.GetSection("WxRedirectUri").Get<string>();
        WeixinApi.Initialize();
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
        if (gzhId != WeixinApi.GZHId)
            return string.Empty;

        return await WeixinHelper.SubscribeAsync(openId, userId);
    }

    private static async Task<string> OnUnsubscribeAsync(
        [FromQuery] string gzhId,
        [FromQuery] string openId,
        [FromQuery] string userId)
    {
        if (gzhId != WeixinApi.GZHId)
            return string.Empty;

        return await WeixinHelper.UnsubscribeAsync(openId, userId);
    }
}