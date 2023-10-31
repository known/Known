namespace Known.Extensions;

public static class ModelExtension
{
    #region User
    public static Task SendMessageAsync(this UserInfo user, Database db, string toUser, string subject, string content, string filePath = null, string bizId = null)
    {
        return user.SendMessageAsync(db, Constants.UMLGeneral, toUser, subject, content, filePath, bizId);
    }

    public static Task SendUrgentMessageAsync(this UserInfo user, Database db, string toUser, string subject, string content, string filePath = null, string bizId = null)
    {
        return user.SendMessageAsync(db, Constants.UMLUrgent, toUser, subject, content, filePath, bizId);
    }

    private static Task SendMessageAsync(this UserInfo user, Database db, string level, string toUser, string subject, string content, string filePath = null, string bizId = null)
    {
        var model = new SysMessage
        {
            UserId = toUser,
            Type = Constants.UMTypeReceive,
            MsgBy = user.Name,
            MsgLevel = level,
            Subject = subject,
            Content = content,
            FilePath = filePath,
            IsHtml = true,
            Status = Constants.UMStatusUnread,
            BizId = bizId
        };
        return db.SaveAsync(model);
    }
    #endregion

    #region Menu
    internal static List<KMenuItem> ToMenuItems(this List<MenuInfo> menus)
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
    #endregion

    #region Module
    internal static List<MenuInfo> ToMenus(this List<SysModule> modules)
    {
        if (modules == null || modules.Count == 0)
            return new List<MenuInfo>();

        return modules.Select(m => new MenuInfo(m.Id, m.Name, m.Icon, m.Description)
        {
            ParentId = m.ParentId,
            Code = m.Code,
            Target = m.Target,
            Sort = m.Sort,
            Buttons = m.Buttons,
            Actions = m.Actions,
            Columns = m.Columns
        }).ToList();
    }

    internal static void RemoveModule(this List<SysModule> modules, string code)
    {
        var module = modules.FirstOrDefault(m => m.Code == code);
        if (module != null)
            modules.Remove(module);
    }
    #endregion

    #region Column
    public static void Add<T>(this List<ColumnInfo> lists, Expression<Func<T, object>> selector, string name = null)
    {
        var property = TypeHelper.Property(selector);
        var attr = property.GetCustomAttribute<ColumnAttribute>(true);
        var column = new ColumnInfo(attr?.Description, property.Name);
        if (!string.IsNullOrWhiteSpace(name))
            column.Name = name;
        lists.Add(column);
    }

    public static void Add<T>(this List<ColumnInfo> lists, Expression<Func<T, object>> selector, ColumnType type)
    {
        var property = TypeHelper.Property(selector);
        var attr = property.GetCustomAttribute<ColumnAttribute>(true);
        var column = new ColumnInfo(attr?.Description, property.Name, type);
        lists.Add(column);
    }
    #endregion
}