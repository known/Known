namespace Known.Extensions;

public static class ModelExtension
{
    #region CodeInfo
    public static List<CodeInfo> ToCodes(this List<CodeInfo> codes, string emptyText = "")
    {
        var infos = new List<CodeInfo>();
        if (!string.IsNullOrWhiteSpace(emptyText))
            infos.Add(new CodeInfo("", emptyText));

        if (codes != null && codes.Count > 0)
            infos.AddRange(codes);

        return infos;
    }
    #endregion

    #region Entity
    internal static List<FieldInfo> GetFields(this EntityInfo info, Language language)
    {
        var infos = new List<FieldInfo>();
        if (info == null)
            return infos;

        foreach (var field in info.Fields)
        {
            infos.Add(new FieldInfo { Id = field.Id, Name = field.Name, Type = field.Type, Required = field.Required });
        }

        if (info.IsFlow)
        {
            infos.Add(GetFieldInfo(language, nameof(FlowEntity.BizStatus), FieldType.Text));
            infos.Add(GetFieldInfo(language, nameof(FlowEntity.ApplyBy), FieldType.Text));
            infos.Add(GetFieldInfo(language, nameof(FlowEntity.ApplyTime), FieldType.DateTime));
            infos.Add(GetFieldInfo(language, nameof(FlowEntity.VerifyBy), FieldType.Text));
            infos.Add(GetFieldInfo(language, nameof(FlowEntity.VerifyTime), FieldType.DateTime));
            infos.Add(GetFieldInfo(language, nameof(FlowEntity.VerifyNote), FieldType.Text));
        }

        infos.Add(GetFieldInfo(language, nameof(EntityBase.CreateBy), FieldType.Text));
        infos.Add(GetFieldInfo(language, nameof(EntityBase.CreateTime), FieldType.DateTime));
        infos.Add(GetFieldInfo(language, nameof(EntityBase.ModifyBy), FieldType.Text));
        infos.Add(GetFieldInfo(language, nameof(EntityBase.ModifyTime), FieldType.DateTime));

        return infos;
    }

    private static FieldInfo GetFieldInfo(Language language, string id, FieldType type) => new() { Id = id, Name = language[id], Type = type };
    #endregion

    #region Menu
    public static List<MenuInfo> ToMenuItems(this List<MenuInfo> menus)
    {
        var items = new List<MenuInfo>();
        if (menus == null || menus.Count == 0)
            return items;

        var tops = menus.Where(m => m.ParentId == "0").OrderBy(m => m.Sort).ToList();
        foreach (var item in tops)
        {
            var menu = new MenuInfo(item);
            items.Add(menu);
            AddChildren(menus, menu);
        }
        return items;
    }

    private static void AddChildren(List<MenuInfo> menus, MenuInfo menu)
    {
        var items = menus.Where(m => m.ParentId == menu.Id).OrderBy(m => m.Sort).ToList();
        if (items == null || items.Count == 0)
            return;

        foreach (var item in items)
        {
            var sub = new MenuInfo(item);
            sub.Parent = menu;
            menu.Children.Add(sub);
            AddChildren(menus, sub);
        }
    }
	#endregion

	#region Module
	internal static List<MenuInfo> ToMenuItems(this List<SysModule> models, bool showRoot = true)
    {
        MenuInfo current = null;
		return models.ToMenuItems(ref current, showRoot);
    }

	internal static List<MenuInfo> ToMenuItems(this List<SysModule> models, ref MenuInfo current, bool showRoot = true)
	{
        MenuInfo root = null;
        var menus = new List<MenuInfo>();
        if (showRoot)
        {
            root = new MenuInfo("0", Config.App.Name, "desktop");
            if (current != null && current.Id == root.Id)
                current = root;

            root.Data = new SysModule { Id = root.Id, Name = root.Name };
            menus.Add(root);
        }
		if (models == null || models.Count == 0)
			return menus;

		var tops = models.Where(m => m.ParentId == "0").OrderBy(m => m.Sort).ToList();
		foreach (var item in tops)
		{
            item.ParentName = Config.App.Name;
			var menu = new MenuInfo(item);
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

	private static void AddChildren(List<SysModule> models, MenuInfo menu, ref MenuInfo current)
	{
		var items = models.Where(m => m.ParentId == menu.Id).OrderBy(m => m.Sort).ToList();
		if (items == null || items.Count == 0)
			return;

		foreach (var item in items)
		{
            item.ParentName = menu.Name;
            var sub = new MenuInfo(item);
			sub.Parent = menu;
			if (current != null && current.Id == sub.Id)
				current = sub;

			menu.Children.Add(sub);
			AddChildren(models, sub, ref current);
		}
	}

	internal static List<MenuInfo> ToMenus(this List<SysModule> modules, bool isAdmin)
    {
        if (modules == null || modules.Count == 0)
            return [];

        return modules.Select(m => new MenuInfo(m, isAdmin)).ToList();
    }

    internal static void RemoveModule(this List<SysModule> modules, string code)
    {
        var module = modules.FirstOrDefault(m => m.Code == code);
        if (module != null)
            modules.Remove(module);
    }
    #endregion

    #region Organization
    internal static List<MenuInfo> ToMenuItems(this List<SysOrganization> models)
    {
        MenuInfo current = null;
        return models.ToMenuItems(ref current);
    }

    internal static List<MenuInfo> ToMenuItems(this List<SysOrganization> models, ref MenuInfo current)
    {
        var menus = new List<MenuInfo>();
        if (models == null || models.Count == 0)
            return menus;

        var tops = models.Where(m => m.ParentId == "0").OrderBy(m => m.Code).ToList();
        foreach (var item in tops)
        {
            var menu = new MenuInfo(item);
			if (current != null && current.Id == menu.Id)
				current = menu;

			menus.Add(menu);
			AddChildren(models, menu, ref current);
        }

        current ??= menus[0];
        return menus;
    }

    private static void AddChildren(List<SysOrganization> models, MenuInfo menu, ref MenuInfo current)
    {
        var items = models.Where(m => m.ParentId == menu.Id).OrderBy(m => m.Code).ToList();
        if (items == null || items.Count == 0)
            return;

        foreach (var item in items)
        {
            item.ParentName = menu.Name;
            var sub = new MenuInfo(item);
            sub.Parent = menu;
			if (current != null && current.Id == sub.Id)
				current = sub;

			menu.Children.Add(sub);
            AddChildren(models, sub, ref current);
        }
    }
    #endregion

    #region File
    public static List<AttachFile> GetAttachFiles(this Dictionary<string, List<FileDataInfo>> files, UserInfo user, string key, string bizType, string bizPath = null) => files?.GetAttachFiles(user, key, new FileFormInfo { BizType = bizType, BizPath = bizPath });

    internal static List<AttachFile> GetAttachFiles(this Dictionary<string, List<FileDataInfo>> files, UserInfo user, string key, FileFormInfo form)
    {
        if (files == null || files.Count == 0)
            return null;

        if (!files.TryGetValue(key, out List<FileDataInfo> value))
            return null;

        var attaches = new List<AttachFile>();
        foreach (var item in value)
        {
            var attach = item.ToAttachFile(user, form);
            attaches.Add(attach);
        }
        return attaches;
    }
    #endregion
}