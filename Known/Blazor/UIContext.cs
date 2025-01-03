﻿namespace Known.Blazor;

/// <summary>
/// UI上下文信息类。
/// </summary>
public class UIContext : Context
{
    /// <summary>
    /// 取得当前上下文菜单信息。
    /// </summary>
    public MenuInfo Current { get; private set; }

    /// <summary>
    /// 取得当前菜单URL。
    /// </summary>
    public string Url { get; internal set; }

    /// <summary>
    /// 取得或设置当前主题。
    /// </summary>
    public string Theme { get; set; }

    /// <summary>
    /// 取得或设置当前用户设置用户系统设置信息对象。
    /// </summary>
    public UserSettingInfo UserSetting { get; set; } = new();

    /// <summary>
    /// 取得或设置当前用户模块表格设置信息列表。
    /// </summary>
    public Dictionary<string, List<TableSettingInfo>> UserTableSettings { get; set; } = [];

    /// <summary>
    /// 取得或设置当前用户权限菜单信息列表。
    /// </summary>
    public List<MenuInfo> UserMenus { get; set; }

    /// <summary>
    /// 根据菜单ID获取菜单信息列表。
    /// </summary>
    /// <param name="menuIds">菜单ID列表。</param>
    /// <returns>菜单信息列表。</returns>
    public List<MenuInfo> GetMenus(List<string> menuIds)
    {
        if (menuIds == null || menuIds.Count == 0)
            return [];

        var menus = new List<MenuInfo>();
        foreach (var menuId in menuIds)
        {
            var menu = UserMenus?.FirstOrDefault(m => m.Name == menuId);
            if (menu != null)
                menus.Add(menu);
        }
        return menus;
    }

    /// <summary>
    /// 判断当前用户是否有按钮权限。
    /// </summary>
    /// <param name="buttonId">按钮ID。</param>
    /// <returns>是否有权限。</returns>
    public bool HasButton(string buttonId)
    {
        var user = CurrentUser;
        if (user == null)
            return false;

        if (user.IsAdmin())
            return true;

        return IsInMenu(Current?.Id, buttonId);
    }

    /// <summary>
    /// 获取用户设置的表格字段列表。
    /// </summary>
    /// <typeparam name="TItem">表格数据行类型。</typeparam>
    /// <param name="table">表格模型对象。</param>
    /// <returns>用户设置的表格字段列表。</returns>
    internal List<ColumnInfo> GetUserTableColumns<TItem>(TableModel<TItem> table) where TItem : class, new()
    {
        var infos = table.Columns.Where(c => c.IsVisible).ToList();
        UserTableSettings.TryGetValue(table.SettingId, out List<TableSettingInfo> settings);
        if (settings == null || settings.Count == 0)
            return infos;

        foreach (var item in infos)
        {
            var setting = settings.FirstOrDefault(c => c.Id == item.Id);
            if (setting != null)
            {
                item.IsVisible = setting.IsVisible;
                item.Width = setting.Width;
                item.Sort = setting.Sort;
            }
        }
        return [.. infos.OrderBy(c => c.Sort)];
    }

    /// <summary>
    /// 用户退出。
    /// </summary>
    public void SignOut()
    {
        CurrentUser = null;
        UserMenus = null;
    }

    internal void SetCurrentMenu(RouteData route)
    {
        Current = GetCurrentMenu(UIConfig.Menus, route);
        if (Current == null)
        {
            var menus = IsMobileApp ? Config.AppMenus : UserMenus;
            Current = GetCurrentMenu(menus, route);
        }
    }

    private MenuInfo GetCurrentMenu(List<MenuInfo> menus, RouteData route)
    {
        if (menus == null || menus.Count == 0)
            return null;

        var menu = menus.FirstOrDefault(m => m.Url == Url);
        if (menu != null)
            return menu;

        var pageRoute = Url.StartsWith("/page/") ? Url.Substring(6) : "";
        return menus.FirstOrDefault(m => m.HasUrl(Url, route, pageRoute));
    }

    private bool IsInMenu(string pageId, string buttonId)
    {
        var menu = UserMenus.FirstOrDefault(m => m.Id == pageId || m.Code == pageId);
        if (menu == null)
            return false;

        var plugin = menu.Plugins?.GetPlugin<EntityPluginInfo>();
        if (plugin == null || plugin.Page == null)
            return false;

        var hasButton = false;
        if (plugin.Page.Tools != null && plugin.Page.Tools.Length > 0)
            hasButton = plugin.Page.Tools.Contains(buttonId);
        else if (plugin.Page.Actions != null && plugin.Page.Actions.Length > 0)
            hasButton = plugin.Page.Actions.Contains(buttonId);
        return hasButton;
    }
}