namespace Known.Extensions;

/// <summary>
/// 数据模型扩展类。
/// </summary>
public static class ModelExtension
{
    #region CodeInfo
    /// <summary>
    /// 往代码表列表中插入空文本字符串。
    /// </summary>
    /// <param name="codes">代码表列表。</param>
    /// <param name="emptyText">空文本字符串，默认空。</param>
    /// <returns>新代码表列表。</returns>
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

    #region ModuleInfo
    internal static List<MenuInfo> ToMenus(this List<ModuleInfo> modules)
    {
        if (modules == null || modules.Count == 0)
            return [];

        return modules.Where(m => m.Enabled).Select(m =>
        {
            var info = CreateMenu(m);
            if (!string.IsNullOrWhiteSpace(m.Url))
            {
                var route = Config.RouteTypes?.FirstOrDefault(r => r.Key == m.Url);
                route ??= Config.RouteTypes?.FirstOrDefault(r => r.Key?.StartsWith(m.Url) == true);
                if (route != null)
                    info.PageType = route.Value.Value;
            }
            return info;
        }).ToList();
    }

    internal static void Resort(this List<ModuleInfo> modules)
    {
        if (modules == null || modules.Count == 0)
            return;

        var index = 1;
        foreach (var item in modules)
        {
            item.Sort = index++;
        }
    }

    private static MenuInfo CreateMenu(ModuleInfo info)
    {
        return new MenuInfo
        {
            Data = info,
            Id = info.Id,
            Name = info.Name,
            Icon = info.Icon,
            ParentId = info.ParentId,
            Type = info.Type,
            Target = info.Target,
            Url = info.Url,
            Sort = info.Sort,
            Enabled = info.Enabled,
            Layout = info.Layout,
            Plugins = info.Plugins
        };
    }

    internal static List<MenuInfo> ToMenuItems(this List<ModuleInfo> models, bool showRoot = true)
    {
        MenuInfo current = null;
        return models.ToMenuItems(ref current, showRoot);
    }

    internal static List<MenuInfo> ToMenuItems(this List<ModuleInfo> models, ref MenuInfo current, bool showRoot = true)
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
            //item.ParentName = Config.App.Name;
            //var menu = item.ToMenuInfo();
            var menu = CreateMenu(item);
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

    private static void AddChildren(List<ModuleInfo> models, MenuInfo menu, ref MenuInfo current)
    {
        var items = models.Where(m => m.ParentId == menu.Id).ToList();
        if (items == null || items.Count == 0)
            return;

        foreach (var item in items)
        {
            //item.ParentName = menu.Name;
            //var sub = item.ToMenuInfo();
            var sub = CreateMenu(item);
            sub.Parent = menu;
            if (current != null && current.Id == sub.Id)
                current = sub;

            menu.Children.Add(sub);
            AddChildren(models, sub, ref current);
        }
    }
    #endregion

    #region MenuInfo
    /// <summary>
    /// 获取系统菜单根节点。
    /// </summary>
    /// <param name="app">系统信息。</param>
    /// <returns></returns>
    public static MenuInfo GetRootMenu(this AppInfo app)
    {
        var root = new MenuInfo { Id = "0", Name = app.Name, Icon = "desktop" };
        root.Data = new ModuleInfo { Id = root.Id, Name = root.Name };
        return root;
    }

    /// <summary>
    /// 将菜单信息列表转成树形结构。
    /// </summary>
    /// <param name="menus">菜单信息列表。</param>
    /// <param name="showRoot">是否显示根节点。</param>
    /// <returns>树形菜单列表。</returns>
    public static List<MenuInfo> ToMenuItems(this List<MenuInfo> menus, bool showRoot = false)
    {
        MenuInfo root = null;
        var items = new List<MenuInfo>();
        if (showRoot)
        {
            root = Config.App.GetRootMenu();
            items.Add(root);
        }
        if (menus == null || menus.Count == 0)
            return items;

        var tops = menus.Where(m => m.ParentId == "0" && m.Target != Constants.Route).OrderBy(m => m.Sort).ToList();
        foreach (var item in tops)
        {
            if (item.Target == Constants.Route)
                continue;

            var menu = CreateMenu(item);
            if (showRoot)
                root.AddChild(menu);
            else
                items.Add(menu);
            AddChildren(menus, menu);
        }
        return items;
    }

    internal static void Resort(this List<MenuInfo> menus)
    {
        if (menus == null || menus.Count == 0)
            return;

        var index = 1;
        foreach (var item in menus)
        {
            item.Sort = index++;
        }
    }

    private static void AddChildren(List<MenuInfo> menus, MenuInfo menu)
    {
        var items = menus.Where(m => m.ParentId == menu.Id).OrderBy(m => m.Sort).ToList();
        if (items == null || items.Count == 0)
            return;

        foreach (var item in items)
        {
            var sub = CreateMenu(item);
            menu.AddChild(sub);
            AddChildren(menus, sub);
        }
    }

    private static MenuInfo CreateMenu(MenuInfo info)
    {
        return new MenuInfo
        {
            Id = info.Id,
            ParentId = info.ParentId,
            Code = info.Code,
            Name = info.Name,
            Icon = info.Icon,
            Description = info.Description,
            Type = info.Type,
            Target = info.Target,
            Url = info.Url,
            Sort = info.Sort,
            Enabled = info.Enabled,
            Layout = info.Layout,
            Plugins = info.Plugins,
            Color = info.Color,
            PageType = info.PageType
        };
    }

    internal static List<CodeInfo> GetAllActions(this MenuInfo info, Language language)
    {
        var codes = new List<CodeInfo>();
        var param = info.GetTablePageParameter();
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
        var param = info.GetTablePageParameter();
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

    #region Language
    internal static string GetFieldName(this Language language, ColumnInfo column, Type type = null)
    {
        if (!string.IsNullOrEmpty(column.Label))
            return column.Label;

        if (!string.IsNullOrEmpty(column.DisplayName))
            return column.DisplayName;

        return language?.GetString(column, type);
    }

    internal static string GetFieldName<TItem>(this Language language, ColumnInfo column)
    {
        return language?.GetFieldName(column, typeof(TItem));
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

    #region File
    /// <summary>
    /// 将附件数据转换成附件类的实例。
    /// </summary>
    /// <param name="file">附件信息。</param>
    /// <param name="user">当前用户信息。</param>
    /// <param name="form">附件表单信息。</param>
    /// <returns></returns>
    public static AttachFile ToAttachFile(this FileDataInfo file, UserInfo user, FileFormInfo form)
    {
        return new AttachFile(file, user, form.BizType, form.BizPath) { Category2 = form.Category };
    }

    /// <summary>
    /// 获取附件字段的文件对象列表。
    /// </summary>
    /// <param name="files">表单的附件字典。</param>
    /// <param name="user">当前用户。</param>
    /// <param name="key">字段名。</param>
    /// <param name="bizType">业务类型。</param>
    /// <param name="bizPath">业务路径。</param>
    /// <returns>文件对象列表。</returns>
    public static List<AttachFile> GetAttachFiles(this Dictionary<string, List<FileDataInfo>> files, UserInfo user, string key, string bizType, string bizPath = null)
    {
        return files?.GetAttachFiles(user, key, new FileFormInfo { BizType = bizType, BizPath = bizPath });
    }

    /// <summary>
    /// 获取附件字段的文件对象列表。
    /// </summary>
    /// <param name="files">表单的附件字典。</param>
    /// <param name="user">当前用户。</param>
    /// <param name="key">字段名。</param>
    /// <param name="form">附件表单对象。</param>
    /// <returns>文件对象列表。</returns>
    public static List<AttachFile> GetAttachFiles(this Dictionary<string, List<FileDataInfo>> files, UserInfo user, string key, FileFormInfo form)
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

    #region Flow
    /// <summary>
    /// 获取工作流步骤项目列表。
    /// </summary>
    /// <param name="info">工作流配置信息。</param>
    /// <returns>步骤项目列表。</returns>
    public static List<ItemModel> GetFlowStepItems(this FlowInfo info)
    {
        if (info == null || info.Steps == null || info.Steps.Count == 0)
            return null;

        return info.Steps.Select(s => new ItemModel(s.Id, s.Name)).ToList();
    }
    #endregion
}