namespace Known.Extensions;

public static class MenuExtension
{
    public static KMenuItem Add(this List<KMenuItem> items, string code, string name, string icon, string description = null)
    {
        var item = new KMenuItem
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

    public static KMenuItem Add<T>(this List<KMenuItem> items, string name, string icon, string description = null)
    {
        var item = new KMenuItem(name, icon, typeof(T), description);
        item.Code = item.Id;
        items.Add(item);
        return item;
    }

    public static List<KMenuItem> ToMenuItems(this List<MenuInfo> menus)
    {
        var items = new List<KMenuItem>();
        if (menus == null || menus.Count == 0)
            return items;

        var tops = menus.Where(m => m.ParentId == "0").OrderBy(m => m.Sort).ToList();
        foreach (var menu in tops)
        {
            var item = KMenuItem.From(menu);
            items.Add(item);
            AddChildren(menus, item);
        }
        return items;
    }

    private static void AddChildren(List<MenuInfo> menus, KMenuItem item)
    {
        var items = menus.Where(m => m.ParentId == item.Id).OrderBy(m => m.Sort).ToList();
        if (items == null || items.Count == 0)
            return;

        foreach (var menu in items)
        {
            var sub = KMenuItem.From(menu);
            sub.Parent = item;
            item.Children.Add(sub);
            AddChildren(menus, sub);
        }
    }

    internal static List<CodeInfo> GetButtonCodes(this MenuInfo menu)
    {
        var items = new List<CodeInfo>();
        if (menu.Buttons != null && menu.Buttons.Count > 0)
            items.AddRange(menu.Buttons.Select(b => GetButton(menu, b, ToolButton.Buttons)));
        if (menu.Actions != null && menu.Actions.Count > 0)
            items.AddRange(menu.Actions.Select(b => GetButton(menu, b, GridAction.Actions)));
        return items;
    }

    private static CodeInfo GetButton(MenuInfo menu, string id, List<ButtonInfo> buttons)
    {
        var code = $"b_{menu.Id}_{id}";
        var button = buttons.FirstOrDefault(b => b.Id == id);
        var name = button != null ? button.Name : id;
        return new CodeInfo(code, name);
    }

    internal static List<CodeInfo> GetColumnCodes(this MenuInfo menu)
    {
        var items = new List<CodeInfo>();
        if (menu.Columns != null && menu.Columns.Count > 0)
            items.AddRange(menu.Columns.Select(b => new CodeInfo($"c_{menu.Id}_{b.Id}", b.Name)));
        return items;
    }

    internal static List<KTreeItem<MenuInfo>> ToTreeItems(this List<MenuInfo> menus)
    {
        var items = new List<KTreeItem<MenuInfo>>();
        if (menus == null || menus.Count == 0)
            return items;

        var tops = menus.Where(m => m.ParentId == "0").OrderBy(m => m.Sort).ToList();
        foreach (var menu in tops)
        {
            var item = new KTreeItem<MenuInfo> { Value = menu, Text = menu.Name, Icon = menu.Icon };
            items.Add(item);
            AddChildren(menus, item);
        }
        return items;
    }

    private static void AddChildren(List<MenuInfo> menus, KTreeItem<MenuInfo> item)
    {
        var items = menus.Where(m => m.ParentId == item.Value.Id).OrderBy(m => m.Sort).ToList();
        if (items == null || items.Count == 0)
            return;

        foreach (var menu in items)
        {
            var sub = new KTreeItem<MenuInfo> { Value = menu, Text = menu.Name, Icon = menu.Icon };
            item.Children.Add(sub);
            AddChildren(menus, sub);
        }
    }
}