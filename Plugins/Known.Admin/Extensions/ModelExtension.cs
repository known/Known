namespace Known.Extensions;

static class ModelExtension
{
    #region Menu
    internal static List<CodeInfo> GetAllActions(this MenuInfo info, Language language)
    {
        var codes = new List<CodeInfo>();
        var param = info.GetAutoPageParameter();
        var page = param?.Page;
        if (page?.Tools != null && page?.Tools.Count > 0)
            codes.AddRange(page?.Tools.Select(b => GetAction(info, language, b)));
        if (page?.Actions != null && page?.Actions.Count > 0)
            codes.AddRange(page?.Actions.Select(b => GetAction(info, language, b)));
        return codes;
    }

    internal static List<CodeInfo> GetAllColumns(this MenuInfo info, Language language)
    {
        var codes = new List<CodeInfo>();
        var param = info.GetAutoPageParameter();
        var page = param?.Page;
        if (page?.Columns != null && page?.Columns.Count > 0)
            codes.AddRange(page?.Columns.Select(c => GetColumn(info, language, c)));
        return codes;
    }

    private static CodeInfo GetAction(MenuInfo menu, Language language, string id)
    {
        var code = $"b_{menu.Id}_{id}";
        var button = Config.Actions.FirstOrDefault(b => b.Id == id);
        var name = button != null ? button.Name : id;
        name = language.GetText("Button", id, name);
        return new CodeInfo(code, name);
    }

    private static CodeInfo GetColumn(MenuInfo menu, Language language, PageColumnInfo info)
    {
        var code = $"c_{menu.Id}_{info.Id}";
        var name = language.GetText("", info.Id, info.Name);
        return new CodeInfo(code, name);
    }
    #endregion

    #region Organization
    internal static List<MenuInfo> ToMenuItems(this List<OrganizationInfo> models)
    {
        MenuInfo current = null;
        return models.ToMenuItems(ref current);
    }

    internal static List<MenuInfo> ToMenuItems(this List<OrganizationInfo> models, ref MenuInfo current)
    {
        var menus = new List<MenuInfo>();
        if (models == null || models.Count == 0)
            return menus;

        var tops = models.Where(m => m.ParentId == "0").OrderBy(m => m.Code).ToList();
        foreach (var item in tops)
        {
            var menu = CreateMenuInfo(item);
            if (current != null && current.Id == menu.Id)
                current = menu;

            menus.Add(menu);
            AddChildren(models, menu, ref current);
        }

        current ??= menus[0];
        return menus;
    }

    private static void AddChildren(List<OrganizationInfo> models, MenuInfo menu, ref MenuInfo current)
    {
        var items = models.Where(m => m.ParentId == menu.Id).OrderBy(m => m.Code).ToList();
        if (items == null || items.Count == 0)
            return;

        foreach (var item in items)
        {
            item.ParentName = menu.Name;
            var sub = CreateMenuInfo(item);
            sub.Parent = menu;
            if (current != null && current.Id == sub.Id)
                current = sub;

            menu.Children.Add(sub);
            AddChildren(models, sub, ref current);
        }
    }

    private static MenuInfo CreateMenuInfo(OrganizationInfo model)
    {
        return new MenuInfo
        {
            Id = model.Id,
            ParentId = model.ParentId,
            Code = model.Code,
            Name = model.Name,
            Data = model
        };
    }
    #endregion
}