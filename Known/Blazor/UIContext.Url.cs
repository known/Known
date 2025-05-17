using AntDesign;

namespace Known.Blazor;

public partial class UIContext
{
    private readonly List<MenuInfo> NavMenus = [];

    internal NavigationManager Navigation { get; set; }
    internal ReuseTabsService TabsService { get; set; }

    /// <summary>
    /// 取得当前菜单URL。
    /// </summary>
    public string Url { get; internal set; }

    /// <summary>
    /// 取得首页URL。
    /// </summary>
    public string HomeUrl => IsMobileApp ? "/app" : "/";

    /// <summary>
    /// 取得当前上下文菜单信息。
    /// </summary>
    public MenuInfo Current { get; private set; }

    /// <summary>
    /// 导航到指定菜单页面。
    /// </summary>
    /// <param name="info">菜单信息。</param>
    public void NavigateTo(MenuInfo info)
    {
        if (info == null || string.IsNullOrWhiteSpace(info.RouteUrl))
            return;

        if (string.IsNullOrWhiteSpace(info.Id))
            throw new ArgumentNullException(nameof(info.Id));

        if (UserSetting.MaxTabCount.HasValue)
        {
            if (TabsService.Pages.Count + 1 > UserSetting.MaxTabCount)
            {
                UI.Info("超过最大标签页数！");
                return;
            }
        }

        SetNavMenu(info);
        Navigation.NavigateTo(info.RouteUrl);
    }

    /// <summary>
    /// 返回到上一个页面。
    /// </summary>
    public void Back()
    {
        if (Current == null || string.IsNullOrWhiteSpace(Current.BackUrl))
            return;

        Navigation?.NavigateTo(Current.BackUrl);
    }

    /// <summary>
    /// 导航到首页。
    /// </summary>
    /// <param name="returnUrl">登录返回地址。</param>
    /// <param name="forceLoad">是否强制刷新。</param>
    public void GoHomePage(string returnUrl = null, bool forceLoad = false)
    {
        var url = returnUrl ?? HomeUrl;
        Navigation.NavigateTo(url, forceLoad);
    }

    /// <summary>
    /// 刷新当前页面。
    /// </summary>
    public void Refresh()
    {
        Navigation.NavigateTo(Current?.RouteUrl, true);
    }

    internal void SetCurrentMenu(RouteData route)
    {
        Current = NavMenus.FirstOrDefault(m => m.HasUrl(Url));
        if (Current != null)
            return;

        var dev = PluginConfig.DevPlugins.FirstOrDefault(m => m.Url == Url);
        if (dev != null)
        {
            Current = dev.ToMenu();
            return;
        }

        var menus = IsMobileApp ? Config.AppMenus : UserMenus;
        if (menus == null || menus.Count == 0)
            return;

        Current = GetCurrentMenu(menus, route);
    }

    private MenuInfo GetCurrentMenu(List<MenuInfo> menus, RouteData route)
    {
        if (menus == null || menus.Count == 0 || string.IsNullOrWhiteSpace(Url))
            return null;

        var index = Url.IndexOf('?');
        var url = index > 0 ? Url[..index] : Url;
        var menu = menus.FirstOrDefault(m => m.Url == url);
        if (menu != null)
            return menu;

        return menus.FirstOrDefault(m => m.HasRoute(url, route));
    }

    private void SetNavMenu(MenuInfo info)
    {
        if (UserMenus?.Exists(m => m.Id == info.Id) == true)
            return;

        if (Config.AppMenus?.Exists(m => m.Id == info.Id) == true)
            return;

        var item = NavMenus.FirstOrDefault(m => m.Id == info.Id);
        if (item != null)
            NavMenus.Remove(item);

        NavMenus.Add(info);
    }
}