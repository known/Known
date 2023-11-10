using Known;
using Known.Razor;
using Microsoft.Extensions.DependencyInjection;

namespace KnownAntDesign;

public static class Extension
{
    public static void AddKAntDesign(this IServiceCollection services)
    {
        //添加模块
        Config.AddModule(typeof(Extension).Assembly);
        //添加UI服务
        services.AddScoped<IUIService, UIService>();
    }
}