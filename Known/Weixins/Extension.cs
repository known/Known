using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Known.Weixins;

public static class Extension
{
    public static void AddKnownWeixin(this IServiceCollection services, IConfiguration configuration)
    {
        WeixinApi.AppId = configuration.GetSection("WxAppId").Get<string>();
        WeixinApi.AppSecret = configuration.GetSection("WxAppSecret").Get<string>();
        WeixinApi.RedirectUri = configuration.GetSection("WxRedirectUri").Get<string>();
        WeixinApi.Initialize();
    }
}