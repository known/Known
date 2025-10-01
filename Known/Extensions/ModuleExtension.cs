namespace Known.Extensions;

/// <summary>
/// 模块信息扩展类。
/// </summary>
public static class ModuleExtension
{
    ///// <summary>
    ///// 添加代码配置的模块列表。
    ///// </summary>
    ///// <param name="modules">模块信息列表。</param>
    ///// <param name="lists">要添加的信息列表。</param>
    ///// <returns></returns>
    //public static List<SysModule> Add(this List<SysModule> modules, List<SysModule> lists)
    //{
    //    if (lists == null || lists.Count == 0)
    //        return modules;

    //    foreach (var module in lists)
    //    {
    //        if (module.Type == nameof(MenuType.Menu) || !modules.Exists(m => m.Url == module.Url))
    //            modules.Add(module);
    //    }
    //    modules = [.. modules.OrderBy(m => m.Sort)];
    //    return modules;
    //}

    ///// <summary>
    ///// 添加一个模块信息。
    ///// </summary>
    ///// <typeparam name="T">模块页面类型。</typeparam>
    ///// <param name="modules">模块列表。</param>
    ///// <param name="parentId">上级ID。</param>
    ///// <param name="sort">排序。</param>
    ///// <returns></returns>
    //public static SysModule AddItem<T>(this List<SysModule> modules, string parentId, int sort)
    //{
    //    var type = typeof(T);
    //    var route = type.GetCustomAttribute<RouteAttribute>();
    //    var plugin = type.GetCustomAttribute<PagePluginAttribute>();
    //    var info = modules.AddItem(parentId, type.FullName, plugin?.Name, plugin?.Icon, sort, route?.Template);
    //    var table = MenuHelper.CreateAutoPage(type);
    //    if (table != null)
    //        info.Plugins.AddPlugin(table);
    //    return info;
    //}

    ///// <summary>
    ///// 添加一个模块信息。
    ///// </summary>
    ///// <param name="modules">模块列表。</param>
    ///// <param name="parentId">上级ID。</param>
    ///// <param name="id">ID。</param>
    ///// <param name="name">名称。</param>
    ///// <param name="icon">图标。</param>
    ///// <param name="sort">排序。</param>
    ///// <param name="url">URL。</param>
    ///// <returns>菜单信息。</returns>
    //public static SysModule AddItem(this List<SysModule> modules, string parentId, string id, string name, string icon, int sort, string url = null)
    //{
    //    var info = new SysModule
    //    {
    //        Id = id,
    //        Type = nameof(MenuType.Menu),
    //        Name = name,
    //        Icon = icon,
    //        ParentId = parentId,
    //        Sort = sort,
    //        Url = url,
    //        IsCode = true
    //    };
    //    if (!string.IsNullOrWhiteSpace(url))
    //    {
    //        info.Type = nameof(MenuType.Link);
    //        info.Target = nameof(LinkTarget.None);
    //    }
    //    modules.Add(info);
    //    return info;
    //}

    /// <summary>
    /// 移除一个模块信息。
    /// </summary>
    /// <param name="modules">模块列表。</param>
    /// <param name="id">模块ID。</param>
    public static void Remove(this List<SysModule> modules, string id)
    {
        if (modules == null || modules.Count == 0)
            return;

        var item = modules.FirstOrDefault(m => m.Id == id);
        if (item != null)
            modules.Remove(item);
    }

    /// <summary>
    /// 改变模块信息的上级ID。
    /// </summary>
    /// <param name="modules">模块列表。</param>
    /// <param name="id">模块ID。</param>
    /// <param name="parentId">上级模块ID。</param>
    public static void ChangeParent(this List<SysModule> modules, string id, string parentId)
    {
        if (modules == null || modules.Count == 0)
            return;

        var item = modules.FirstOrDefault(m => m.Id == id);
        if (item != null)
            item.ParentId = parentId;
    }

    ///// <summary>
    ///// 将模块信息列表转成菜单信息列表。
    ///// </summary>
    ///// <param name="modules">模块信息列表。</param>
    ///// <returns>菜单信息列表。</returns>
    //public static List<MenuInfo> ToMenus(this List<SysModule> modules)
    //{
    //    if (modules == null || modules.Count == 0)
    //        return [];

    //    return modules.Where(m => m.Enabled).Select(m =>
    //    {
    //        var info = m.ToMenuInfo();
    //        if (!string.IsNullOrWhiteSpace(m.Url))
    //        {
    //            var route = Config.RouteTypes?.FirstOrDefault(r => r.Key == m.Url);
    //            route ??= Config.RouteTypes?.FirstOrDefault(r => r.Key?.StartsWith(m.Url) == true);
    //            if (route != null)
    //                info.PageType = route.Value.Value;
    //        }
    //        return info;
    //    }).ToList();
    //}

    /// <summary>
    /// 重新给模块列表排序。
    /// </summary>
    /// <param name="modules">模块列表。</param>
    public static void Resort(this List<SysModule> modules)
    {
        if (modules == null || modules.Count == 0)
            return;

        var index = 1;
        foreach (var item in modules)
        {
            item.Sort = index++;
        }
    }
}