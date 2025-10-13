namespace Known.Extensions;

static class ModelExtension
{
    #region Module
    internal static void RemoveModule(this List<SysModule1> modules, string code)
    {
        var module = modules.FirstOrDefault(m => m.Code == code);
        if (module != null)
            modules.Remove(module);
    }

    internal static List<MenuInfo> ToMenuItems(this List<SysModule1> models, bool showRoot = true)
    {
        MenuInfo current = null;
        return models.ToMenuItems(ref current, showRoot);
    }

    internal static List<MenuInfo> ToMenuItems(this List<SysModule1> models, ref MenuInfo current, bool showRoot = true)
    {
        MenuInfo root = null;
        var menus = new List<MenuInfo>();
        if (showRoot)
        {
            root = root = Config.App.GetRootMenu();
            if (current != null && current.Id == root.Id)
                current = root;
            menus.Add(root);
        }
        if (models == null || models.Count == 0)
            return menus;

        var tops = models.Where(m => m.ParentId == "0").ToList();
        foreach (var item in tops)
        {
            item.ParentName = Config.App.Name;
            var menu = item.ToMenuInfo();
            if (current != null && current.Id == menu.Id)
                current = menu;

            if (showRoot)
                root.Children.Add(menu);
            else
                menus.Add(menu);
            AddChildren(models, menu, ref current);
        }

        current ??= menus[0];
        return menus;
    }

    private static void AddChildren(List<SysModule1> models, MenuInfo menu, ref MenuInfo current)
    {
        var items = models.Where(m => m.ParentId == menu.Id).ToList();
        if (items == null || items.Count == 0)
            return;

        foreach (var item in items)
        {
            item.ParentName = menu.Name;
            var sub = item.ToMenuInfo();
            sub.Parent = menu;
            if (current != null && current.Id == sub.Id)
                current = sub;

            menu.Children.Add(sub);
            AddChildren(models, sub, ref current);
        }
    }
    #endregion
}