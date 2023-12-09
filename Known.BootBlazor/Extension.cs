using BootstrapBlazor.Components;
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
        services.AddScoped<UIService>();
    }

    internal static List<BootstrapBlazor.Components.MenuItem> ToSideMenuItems(this List<MenuItem> menus)
    {
        var items = new List<BootstrapBlazor.Components.MenuItem>();
        if (menus == null || menus.Count == 0)
            return items;

        foreach (var menu in menus)
        {
            var item = new BootstrapBlazor.Components.MenuItem(menu.Name, icon: menu.Icon);
            item.Items = menu.Children.Select(m => new BootstrapBlazor.Components.MenuItem(m.Name, icon: m.Icon));
            items.Add(item);
        }

        return items;
    }

    internal static IEnumerable<SelectedItem> ToSelectedItems(this List<CodeInfo> codes, Action<SelectedItem> action = null)
    {
        if (codes == null || codes.Count == 0)
            return null;

        return codes.Select(a =>
        {
            var option = new SelectedItem
            {
                Value = a.Code,
                Text = a.Name
            };
            action?.Invoke(option);
            return option;
        });
    }

    internal static Color ToColor(this ActionInfo action)
    {
        if (action.Style == "primary")
            return Color.Primary;
        else if (action.Style == "danger")
            return Color.Danger;

        return Color.Primary;
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