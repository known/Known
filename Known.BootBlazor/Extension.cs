using Microsoft.Extensions.DependencyInjection;

namespace Known.BootBlazor;

public static class Extension
{
    public static void AddKnownBootstrap(this IServiceCollection services, Action<BootstrapOption> action = null)
    {
        //添加BootstrapBlazor
        services.AddBootstrapBlazor();
        services.AddScoped<IUIService, UIService>();

        BootConfig.Option = new BootstrapOption();
        action?.Invoke(BootConfig.Option);

        Config.AddModule(typeof(Extension).Assembly);
    }
}

public class BootstrapOption
{
    public RenderFragment Footer { get; set; }
}

class BootConfig
{
    public static BootstrapOption Option { get; set; }
}