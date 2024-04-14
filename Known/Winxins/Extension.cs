using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Known.Winxins;

public static class Extension
{
    public static void AddKnownWeixin(this IServiceCollection services, IConfiguration configuration)
    {
        WeixinHelper.AppId = configuration.GetSection("WxAppId").Get<string>();
        WeixinHelper.AppSecret = configuration.GetSection("WxAppSecret").Get<string>();
        WeixinHelper.RedirectUri = configuration.GetSection("WxRedirectUri").Get<string>();

        WeixinApi.Initialize();
    }
}