namespace Known.Blazor;

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
    /// 取得或设置上下文UI服务对象。
    /// </summary>
    public IUIService UI { get; set; }

    /// <summary>
    /// 取得或设置当前菜单URL。
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 取得或设置当前主题。
    /// </summary>
    public string Theme { get; set; }

    /// <summary>
    /// 取得或设置系统信息对象。
    /// </summary>
    public SystemInfo System { get; set; }

    /// <summary>
    /// 取得当前用户设置信息对象。
    /// </summary>
    public SettingInfo UserSetting { get; internal set; } = new();

    /// <summary>
    /// 取得当前用户权限菜单信息列表。
    /// </summary>
    public List<MenuInfo> UserMenus { get; internal set; }

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

        if (user.IsAdmin)
            return true;

        return IsInMenu(Current?.Id, buttonId);
    }

    private bool IsInMenu(string pageId, string buttonId)
    {
        var menu = UserMenus.FirstOrDefault(m => m.Id == pageId || m.Code == pageId);
        if (menu == null)
            return false;

        var hasButton = false;
        if (menu.Tools != null && menu.Tools.Count > 0)
            hasButton = menu.Tools.Contains(buttonId);
        else if (menu.Actions != null && menu.Actions.Count > 0)
            hasButton = menu.Actions.Contains(buttonId);
        return hasButton;
    }

    internal void SignOut()
    {
        CurrentUser = null;
        UserMenus = null;
    }

    internal void SetCurrentMenu(string pageRoute = "")
    {
        Current = UIConfig.Menus.FirstOrDefault(m => m.Url == Url || m.Url == $"/{pageRoute}" || m.Id == pageRoute);
        if (Current == null)
        {
            var menus = IsMobile ? Config.AppMenus : UserMenus;
            Current = menus?.FirstOrDefault(m => m.Url == Url || m.Url == $"/{pageRoute}" || m.Id == pageRoute);
        }
    }
}