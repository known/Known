using Microsoft.Extensions.DependencyInjection;

namespace Known.AntBlazor;

public static class Extension
{
    public static void AddKnownAntDesign(this IServiceCollection services, Action<AntDesignOption> action = null)
    {
        //添加AntDesign
        services.AddAntDesign();
        services.AddScoped<IUIService, UIService>();

        AntConfig.Option = new AntDesignOption();
        action?.Invoke(AntConfig.Option);

        Config.AddModule(typeof(Extension).Assembly);

        UIConfig.Sizes = [
            new ActionInfo { Id = "Default", Style = "ant-design-blazor", Url = "_content/AntDesign/css/ant-design-blazor.css" },
            new ActionInfo { Id = "Compact", Style = "ant-design-blazor", Url = "_content/AntDesign/css/ant-design-blazor.compact.css" }
        ];
        UIConfig.Icons["AntDesign"] = typeof(IconType.Outline).GetProperties()
            .Select(x => (string)x.GetValue(null))
            .Where(x => x is not null)
            .ToList();
    }
}

public class AntDesignOption
{
    public bool ShowFooter { get; set; }
    public RenderFragment Footer { get; set; }
}

class AntConfig
{
    public static AntDesignOption Option { get; set; }
}