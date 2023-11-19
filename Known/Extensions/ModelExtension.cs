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
		var root = new MenuItem("0", Config.App.Name, "desktop");
        root.Data = new SysModule { Id = root.Id, Name = root.Name };
        menus.Add(root);
		if (models == null || models.Count == 0)
			return menus;

		var tops = models.Where(m => m.ParentId == "0").OrderBy(m => m.Sort).ToList();
		foreach (var item in tops)
		{
            item.ParentName = Config.App.Name;
			var menu = MenuItem.From(item);
			if (current != null && current.Id == menu.Id)
				current = menu;

			root.Children.Add(menu);
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
            item.ParentName = menu.Name;
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

    #region Organization
    internal static List<MenuItem> ToMenuItems(this List<SysOrganization> models)
    {
        MenuItem current = null;
        return models.ToMenuItems(ref current);
    }

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

    #region File
    public static List<AttachFile> GetAttachFiles(this Dictionary<string, List<IAttachFile>> files, UserInfo user, string key, string typePath) => files?.GetAttachFiles(user, key, new FileFormInfo { BizType = typePath });

    internal static List<AttachFile> GetAttachFiles(this Dictionary<string, List<IAttachFile>> files, UserInfo user, string key, FileFormInfo form)
    {
        if (files == null || files.Count == 0)
            return null;

        if (!files.TryGetValue(key, out List<IAttachFile> value))
            return null;

        var attaches = new List<AttachFile>();
        foreach (var item in value)
        {
            var attach = new AttachFile(item, user, form.BizType, form.BizPath) { Category2 = form.Category };
            attaches.Add(attach);
        }
        return attaches;
    }
    #endregion
}