﻿using AntDesign;

namespace Known;

/// <summary>
/// 框架配置扩展类。
/// </summary>
public static class Extension
{
    /// <summary>
    /// 添加Known框架配置。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">系统配置方法。</param>
    public static void AddKnown(this IServiceCollection services, Action<AppInfo> action = null)
    {
        Config.StartTime = DateTime.Now;
        action?.Invoke(Config.App);

        if (Config.App.Type == AppType.WebApi)
            return;

        Config.AddApp();
        services.AddAntDesign();

        services.AddScoped<Context>();
        services.AddScoped<UIContext>();
        services.AddScoped<UIService>();
        services.AddScoped<JSService>();
        services.AddScoped<IAuthStateProvider, AuthStateProvider>();
        services.AddScoped<IPlatformService, PlatformService>();
        services.AddScoped<IAutoService, AutoService>();
        services.AddScoped(typeof(IEntityService<>), typeof(EntityService<>));

        KStyleSheet.AddStyle("_content/AntDesign/css/ant-design-blazor.css");
        KStyleSheet.AddStyle("_content/Known/css/theme/default.css");
        KStyleSheet.AddStyle("_content/Known/css/size/default.css");
        KStyleSheet.AddStyle("_content/Known/css/web.css");

        KScript.AddScript("_content/AntDesign/js/ant-design-blazor.js");
        KScript.AddScript("_content/Known/js/libs/jquery.js");
        KScript.AddScript("_content/Known/js/libs/pdfobject.js");
        //KScript.AddScript("_content/Known/js/libs/highcharts.js");
        KScript.AddScript("_content/Known/js/libs/barcode.js");
        KScript.AddScript("_content/Known/js/libs/qrcode.js");
        KScript.AddScript("_content/Known/js/libs/prism.js");
        KScript.AddScript("_content/Known/js/web.js");

        UIConfig.Icons["AntDesign"] = typeof(IconType.Outline).GetProperties().Select(x => (string)x.GetValue(null)).Where(x => x is not null).ToList();
        var content = Utils.GetResource(typeof(Extension).Assembly, "IconFA");
        if (!string.IsNullOrWhiteSpace(content))
        {
            var lines = content.Split([.. Environment.NewLine]);
            UIConfig.Icons["FontAwesome"] = lines.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => $"fa fa-{l}").ToList();
        }
        UIConfig.Sizes = [
            new ActionInfo { Id = "Default", Style = "size", Url = "_content/Known/css/size/default.css" },
            new ActionInfo { Id = "Compact", Style = "size", Url = "_content/Known/css/size/compact.css" }
        ];
    }

    /// <summary>
    /// 添加Known框架客户端配置。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">客户端配置选项委托。</param>
    public static void AddKnownClient(this IServiceCollection services, Action<ClientOption> action = null)
    {
        Config.IsClient = true;
        action?.Invoke(ClientOption.Instance);

        var option = ClientOption.Instance;
        services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(option.BaseAddress) });
        services.AddScoped<IAuthStateProvider, AuthStateProvider>();
        services.AddScoped<IPlatformService, PlatformClient>();
        services.AddScoped<IAutoService, AutoClient>();
        services.AddScoped(typeof(IEntityService<>), typeof(EntityClient<>));

        if (option.InterceptorType != null)
        {
            foreach (var type in Config.ApiTypes)
            {
                //Console.WriteLine(type.Name);
                var interceptorType = option.InterceptorType.Invoke(type);
                if (interceptorType == null)
                    continue;

                services.AddScoped(interceptorType);
                services.AddScoped(type, provider =>
                {
                    var interceptor = provider.GetRequiredService(interceptorType);
                    return option.InterceptorProvider?.Invoke(type, interceptor);
                });
            }
        }
    }

    /// <summary>
    /// 添加Known框架简易ORM数据访问组件。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">ORM配置选项委托。</param>
    public static void AddKnownData(this IServiceCollection services, Action<DatabaseOption> action = null)
    {
        action?.Invoke(DatabaseOption.Instance);
        services.AddScoped<Database>();
    }
}