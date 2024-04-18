using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

namespace Known.Weixins;

public static class Extension
{
    public static void AddKnownWeixin(this IServiceCollection services, IConfiguration configuration)
    {
        WeixinApi.GZHId = configuration.GetSection("WxGZHId").Get<string>();
        WeixinApi.AppId = configuration.GetSection("WxAppId").Get<string>();
        WeixinApi.AppSecret = configuration.GetSection("WxAppSecret").Get<string>();
        WeixinApi.RedirectUri = configuration.GetSection("WxRedirectUri").Get<string>();
        WeixinApi.Initialize();
    }

    public static void UseKnownWeixin(this WebApplication app)
    {
        app.MapGet("/weixin/auth", OnAuthorizeAsync);
        app.MapGet("/weixin/subs", OnSubscribeAsync);
        app.MapGet("/weixin/unsubs", OnUnsubscribeAsync);
    }

    private static async Task OnAuthorizeAsync([FromQuery] string token, [FromQuery] string code)
    {
        await WeixinService.AuthorizeAsync(token, code);
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

        return await WeixinService.SubscribeAsync(openId, userId);
    }

    private static async Task<string> OnUnsubscribeAsync(
        [FromQuery] string gzhId,
        [FromQuery] string openId,
        [FromQuery] string userId)
    {
        if (gzhId != WeixinApi.GZHId)
            return string.Empty;

        return await WeixinService.UnsubscribeAsync(openId, userId);
    }
}