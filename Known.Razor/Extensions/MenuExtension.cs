namespace Known.Razor.Extensions;

public static class MenuExtension
{
    public static MenuItem Add(this List<MenuItem> items, string code, string name, string icon, string description = null)
    {
        var item = new MenuItem
        {
            Id = code,
            Code = code,
            Name = name,
            Icon = icon,
            Description = description
        };
        items.Add(item);
        return item;
    }

    public static MenuItem Add<T>(this List<MenuItem> items, string name, string icon, string description = null)
    {
        var item = new MenuItem(name, icon, typeof(T), description);
        item.Code = item.Id;
        items.Add(item);
        return item;
    }

    internal static List<MenuItem> ToMenuItems(this List<MenuInfo> menus)
    {
        var items = new List<MenuItem>();
        if (menus == null || menus.Count == 0)
            return items;

        var tops = menus.Where(m => m.ParentId == "0").OrderBy(m => m.Sort).ToList();
        foreach (var menu in tops)
        {
            var item = MenuItem.From(menu);
            items.Add(item);
            AddChildren(menus, item);
        }
        return items;
    }

    private static void AddChildren(List<MenuInfo> menus, MenuItem item)
    {
        var items = menus.Where(m => m.ParentId == item.Id).OrderBy(m => m.Sort).ToList();
        if (items == null || items.Count == 0)
            return;

        foreach (var menu in items)
        {
            var sub = MenuItem.From(menu);
            item.Children.Add(sub);
            AddChildren(menus, sub);
        }
    }

    internal static List<TreeItem<MenuInfo>> ToTreeItems(this List<MenuInfo> menus)
    {
        var items = new List<TreeItem<MenuInfo>>();
        if (menus == null || menus.Count == 0)
            return items;

        var tops = menus.Where(m => m.ParentId == "0").OrderBy(m => m.Sort).ToList();
        foreach (var menu in tops)
        {
            var item = new TreeItem<MenuInfo> { Value = menu, Text = menu.Name, Icon = menu.Icon };
            items.Add(item);
            AddChildren(menus, item);
        }
        return items;
    }

    private static void AddChildren(List<MenuInfo> menus, TreeItem<MenuInfo> item)
    {
        var items = menus.Where(m => m.ParentId == item.Value.Id).OrderBy(m => m.Sort).ToList();
        if (items == null || items.Count == 0)
            return;

        foreach (var menu in items)
        {
            var sub = new TreeItem<MenuInfo> { Value = menu, Text = menu.Name, Icon = menu.Icon };
            item.Children.Add(sub);
            AddChildren(menus, sub);
        }
    }
}