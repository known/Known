namespace Known.BootBlazor.Extensions;

static class OptionExtension
{
    internal static List<MenuItem> ToSideMenuItems(this List<MenuInfo> menus, Context context)
    {
        var items = new List<MenuItem>();
        if (menus == null || menus.Count == 0)
            return items;

        foreach (var menu in menus)
        {
            menu.Icon = "fa fa-desktop";
            var menuName = context.Language.GetString(menu);
            var item = new MenuItem(menuName, icon: menu.Icon) { Id = menu.Id };
            item.Items = menu.Children.Select(m =>
            {
                m.Icon = "fa fa-bars";
                var mName = context.Language.GetString(m);
                return new BootstrapBlazor.Components.MenuItem(mName, icon: m.Icon) { Id = m.Id };
            });
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
            var option = new SelectedItem { Value = a.Code, Text = a.Name };
            action?.Invoke(option);
            return option;
        });
    }

    internal static Color ToColor(this ActionInfo action) => action.Style.ToColor();
    internal static Color ToColor(this string style)
    {
        if (style == "primary")
            return Color.Primary;
        else if (style == "danger")
            return Color.Danger;

        return Color.Primary;
    }
}