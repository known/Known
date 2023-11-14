using Known.Entities;

namespace Known.Extensions;

public static class ModelExtension
{
    #region Menu
    internal static List<MenuItem> ToMenuItems(this List<MenuInfo> menus)
    {
        var items = new List<MenuItem>();
        if (menus == null || menus.Count == 0)
            return items;

        var tops = menus.Where(m => m.ParentId == "0").OrderBy(m => m.Sort).ToList();
        foreach (var item in tops)
        {
            var menu = MenuItem.From(item);
            items.Add(menu);
            AddChildren(menus, menu);
        }
        return items;
    }

    private static void AddChildren(List<MenuInfo> menus, MenuItem menu)
    {
        var items = menus.Where(m => m.ParentId == menu.Id).OrderBy(m => m.Sort).ToList();
        if (items == null || items.Count == 0)
            return;

        foreach (var item in items)
        {
            var sub = MenuItem.From(item);
            sub.Parent = menu;
            menu.Children.Add(sub);
            AddChildren(menus, sub);
        }
    }

    public static List<CodeInfo> GetActionCodes(this MenuInfo menu)
    {
        var actions = ActionInfo.Actions;
        var codes = new List<CodeInfo>();
        if (menu.Buttons != null && menu.Buttons.Count > 0)
            codes.AddRange(menu.Buttons.Select(b => GetButton(menu, b, actions)));
        if (menu.Actions != null && menu.Actions.Count > 0)
            codes.AddRange(menu.Actions.Select(b => GetButton(menu, b, actions)));
        return codes;
    }

    private static CodeInfo GetButton(MenuInfo menu, string id, List<ActionInfo> buttons)
    {
        var code = $"b_{menu.Id}_{id}";
        var button = buttons.FirstOrDefault(b => b.Id == id);
        var name = button != null ? button.Name : id;
        return new CodeInfo(code, name);
    }

    public static List<CodeInfo> GetColumnCodes(this MenuInfo menu)
    {
        var codes = new List<CodeInfo>();
        if (menu.Columns != null && menu.Columns.Count > 0)
            codes.AddRange(menu.Columns.Select(b => new CodeInfo($"c_{menu.Id}_{b.Id}", b.Name)));
        return codes;
    }
	#endregion

	#region Module
	internal static List<MenuItem> ToMenuItems(this List<SysModule> models)
    {
        MenuItem current = null;
		return models.ToMenuItems(ref current);
    }

	internal static List<MenuItem> ToMenuItems(this List<SysModule> models, ref MenuItem current)
	{
		var menus = new List<MenuItem>();
		if (models == null || models.Count == 0)
			return menus;

		var tops = models.Where(m => m.ParentId == "0").OrderBy(m => m.Sort).ToList();
		foreach (var item in tops)
		{
			var menu = MenuItem.From(item);
			if (current != null && current.Id == menu.Id)
				current = menu;

			menus.Add(menu);
			AddChildren(models, menu, ref current);
		}
		return menus;
	}

	private static void AddChildren(List<SysModule> models, MenuItem menu, ref MenuItem current)
	{
		var items = models.Where(m => m.ParentId == menu.Id).OrderBy(m => m.Sort).ToList();
		if (items == null || items.Count == 0)
			return;

		foreach (var item in items)
		{
			var sub = MenuItem.From(item);
			sub.Parent = menu;
			if (current != null && current.Id == sub.Id)
				current = sub;

			menu.Children.Add(sub);
			AddChildren(models, sub, ref current);
		}
	}

	internal static List<MenuInfo> ToMenus(this List<SysModule> modules)
    {
        if (modules == null || modules.Count == 0)
            return [];

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

    //#region Column
    //public static void Add<T>(this List<ColumnInfo> lists, Expression<Func<T, object>> selector, string name = null)
    //{
    //    var property = TypeHelper.Property(selector);
    //    var attr = property.GetCustomAttribute<ColumnAttribute>(true);
    //    var column = new ColumnInfo(attr?.Description, property.Name);
    //    if (!string.IsNullOrWhiteSpace(name))
    //        column.Name = name;
    //    lists.Add(column);
    //}

    //public static void Add<T>(this List<ColumnInfo> lists, Expression<Func<T, object>> selector, ColumnType type)
    //{
    //    var property = TypeHelper.Property(selector);
    //    var attr = property.GetCustomAttribute<ColumnAttribute>(true);
    //    var column = new ColumnInfo(attr?.Description, property.Name, type);
    //    lists.Add(column);
    //}
    //#endregion

    #region Organization
    internal static List<MenuItem> ToMenuItems(this List<SysOrganization> models, ref MenuItem current)
    {
        var menus = new List<MenuItem>();
        if (models == null || models.Count == 0)
            return menus;

        var tops = models.Where(m => m.ParentId == "0").OrderBy(m => m.Code).ToList();
        foreach (var item in tops)
        {
            var menu = MenuItem.From(item);
			if (current != null && current.Id == menu.Id)
				current = menu;

			menus.Add(menu);
			AddChildren(models, menu, ref current);
        }
        return menus;
    }

    private static void AddChildren(List<SysOrganization> models, MenuItem menu, ref MenuItem current)
    {
        var items = models.Where(m => m.ParentId == menu.Id).OrderBy(m => m.Code).ToList();
        if (items == null || items.Count == 0)
            return;

        foreach (var item in items)
        {
            item.ParentName = menu.Name;
            var sub = MenuItem.From(item);
            sub.Parent = menu;
			if (current != null && current.Id == sub.Id)
				current = sub;

			menu.Children.Add(sub);
            AddChildren(models, sub, ref current);
        }
    }
    #endregion

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
}