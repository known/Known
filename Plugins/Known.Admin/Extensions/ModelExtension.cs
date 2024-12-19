namespace Known.Extensions;

static class ModelExtension
{
    #region Module
    internal static void RemoveModule(this List<SysModule> modules, string code)
    {
        var module = modules.FirstOrDefault(m => m.Code == code);
        if (module != null)
            modules.Remove(module);
    }

    internal static List<ModuleInfo> ToModuleLists(this List<SysModule> models)
    {
        return models?.Select(m => m.ToModuleInfo()).ToList();
    }

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
            root = new MenuInfo { Id = "0", Name = Config.App.Name, Icon = "desktop" };
            if (current != null && current.Id == root.Id)
                current = root;

            root.Data = new ModuleInfo { Id = root.Id, Name = root.Name };
            menus.Add(root);
        }
        if (models == null || models.Count == 0)
            return menus;

        var tops = models.Where(m => m.ParentId == "0").OrderBy(m => m.Sort).ToList();
        foreach (var item in tops)
        {
            item.ParentName = Config.App.Name;
            var menu = CreateMenuInfo(item);
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
            var sub = CreateMenuInfo(item);
            sub.Parent = menu;
            if (current != null && current.Id == sub.Id)
                current = sub;

            menu.Children.Add(sub);
            AddChildren(models, sub, ref current);
        }
    }

    private static MenuInfo CreateMenuInfo(SysModule module)
    {
        module.LoadData();
        var menu = new MenuInfo();
        menu.Data = module;
        menu.Id = module.Id;
        menu.Name = module.Name;
        menu.Icon = module.Icon;
        menu.Description = module.Description;
        menu.ParentId = module.ParentId;
        menu.Code = module.Code;
        menu.Target = module.Target;
        menu.Url = module.Url;
        menu.Sort = module.Sort;
        menu.Model = DataHelper.ToEntity(module.EntityData);
        menu.Page = module.Page;
        menu.Form = module.Form;
        menu.Tools = module.Buttons;
        menu.Actions = module.Actions;
        menu.Columns = module.Columns;
        return menu;
    }
    #endregion

    #region MenuInfo
    internal static List<CodeInfo> GetAllActions(this MenuInfo info)
    {
        var codes = new List<CodeInfo>();
        if (info.Tools != null && info.Tools.Count > 0)
            codes.AddRange(info.Tools.Select(b => GetAction(info, b)));
        if (info.Actions != null && info.Actions.Count > 0)
            codes.AddRange(info.Actions.Select(b => GetAction(info, b)));
        return codes;
    }

    internal static List<CodeInfo> GetAllColumns(this MenuInfo info)
    {
        var codes = new List<CodeInfo>();
        if (info.Columns != null && info.Columns.Count > 0)
            codes.AddRange(info.Columns.Select(b => new CodeInfo($"c_{info.Id}_{b.Id}", b.Name)));
        return codes;
    }

    private static CodeInfo GetAction(MenuInfo menu, string id)
    {
        var code = $"b_{menu.Id}_{id}";
        var button = Config.Actions.FirstOrDefault(b => b.Id == id);
        var name = button != null ? button.Name : id;
        return new CodeInfo(code, name);
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
            var menu = CreateMenuInfo(item);
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
            var sub = CreateMenuInfo(item);
            sub.Parent = menu;
            if (current != null && current.Id == sub.Id)
                current = sub;

            menu.Children.Add(sub);
            AddChildren(models, sub, ref current);
        }
    }

    private static MenuInfo CreateMenuInfo(SysOrganization model)
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

    #region User
    /// <summary>
    /// 发送站内消息。
    /// </summary>
    /// <param name="user">用户对象。</param>
    /// <param name="db">数据库对象。</param>
    /// <param name="toUser">站内收件人。</param>
    /// <param name="subject">消息主题。</param>
    /// <param name="content">消息内容。</param>
    /// <param name="isUrgent">是否紧急。</param>
    /// <param name="filePath">附件路径。</param>
    /// <param name="bizId">关联业务数据ID。</param>
    /// <returns></returns>
    public static Task SendMessageAsync(this UserInfo user, Database db, string toUser, string subject, string content, bool isUrgent = false, string filePath = null, string bizId = null)
    {
        var level = isUrgent ? Constant.UMLUrgent : Constant.UMLGeneral;
        return SendMessageAsync(db, user, level, toUser, subject, content, filePath, bizId);
    }

    private static Task SendMessageAsync(Database db, UserInfo user, string level, string toUser, string subject, string content, string filePath = null, string bizId = null)
    {
        var model = new SysMessage
        {
            UserId = toUser,
            Type = Constant.UMTypeReceive,
            MsgBy = user.Name,
            MsgLevel = level,
            Subject = subject,
            Content = content,
            FilePath = filePath,
            IsHtml = true,
            Status = Constant.UMStatusUnread,
            BizId = bizId
        };
        return db.SaveAsync(model);
    }
    #endregion'
}