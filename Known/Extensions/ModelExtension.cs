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

    #region Module
    internal static List<MenuInfo> ToMenus(this List<ModuleInfo> modules)
    {
        if (modules == null || modules.Count == 0)
            return [];

        return modules.Where(m => m.Enabled).Select(m =>
        {
            var info = new MenuInfo(m);
            if (!string.IsNullOrWhiteSpace(m.Url))
            {
                var route = Config.RouteTypes?.FirstOrDefault(r => r.Key?.StartsWith(m.Url) == true);
                if (route != null)
                    info.PageType = route.Value.Value;
            }
            return info;
        }).ToList();
    }
    #endregion

    #region Menu
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

            var menu = new MenuInfo(item);
            if (showRoot)
                root.Children.Add(menu);
            else
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