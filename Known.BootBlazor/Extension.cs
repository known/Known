using Known.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Known.BootBlazor;

public static class Extension
{
    public static void AddKnownBootstrap(this IServiceCollection services, Action<BootstrapOption> action = null)
    {
        //添加BootstrapBlazor
        services.AddBootstrapBlazor();

        BootConfig.Option = new BootstrapOption();
        action?.Invoke(BootConfig.Option);

        Config.AddModule(typeof(Extension).Assembly);
        services.AddScoped<IUIService, UIService>();
    }

    internal static List<BootstrapBlazor.Components.MenuItem> ToSideMenuItems(this List<MenuItem> menus)
    {
        var items = new List<BootstrapBlazor.Components.MenuItem>();
        if (menus == null || menus.Count == 0)
            return items;

        foreach (var menu in menus)
        {
            var item = new BootstrapBlazor.Components.MenuItem(menu.Name, icon: menu.Icon);
            items.Add(item);

            foreach (var sub in menu.Children)
            {
                var subItem = new BootstrapBlazor.Components.MenuItem(sub.Name, icon: sub.Icon);
                item.Items.ToList().Add(subItem);
            }
        }

        return items;
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