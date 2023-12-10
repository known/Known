using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Known.BulmaBlazor;

public static class Extension
{
    public static void AddKnownBulma(this IServiceCollection services, Action<BulmaOption> action = null)
    {
        //添加BulmaRazor
        services.AddBulmaRazor();

        BulmaConfig.Option = new BulmaOption();
        action?.Invoke(BulmaConfig.Option);

        Config.AddModule(typeof(Extension).Assembly);
        services.AddScoped<UIService>();
    }
}

public class BulmaOption
{
    public RenderFragment Footer { get; set; }
}

class BulmaConfig
{
    public static BulmaOption Option { get; set; }
}