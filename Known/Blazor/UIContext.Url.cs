using AntDesign;

namespace Known.Blazor;

public partial class UIContext
{
    private readonly List<MenuInfo> NavMenus = [];

    internal bool IsInitial { get; set; }
    internal bool IsNotifyHub { get; set; }
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
    /// 导航到指定类型的页面。
    /// </summary>
    /// <typeparam name="T">页面类型。</typeparam>
    public void NavigateTo<T>()
    {
        var id = typeof(T).Name;
        var info = UserMenus?.FirstOrDefault(m => m.Id == id);
        info ??= Config.AppMenus.FirstOrDefault(m => m.Id == id);
        NavigateTo(info);
    }

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

        if (UserSetting.MaxTabCount.HasValue && !IsMobileApp)
        {
            if (TabsService.Pages.Count + 1 > UserSetting.MaxTabCount)
            {
                UI.Info(Language.OverMaxTabCount);
                return;
            }
        }

        SetNavMenu(info);
        NavigateTo(info.RouteUrl);
    }

    /// <summary>
    /// 导航到指定地址页面。
    /// </summary>
    /// <param name="url">导航页面地址。</param>
    /// <param name="forceLoad">是否强制刷新。</param>
    public void NavigateTo(string url, bool forceLoad = false)
    {
        Navigation.NavigateTo(url, forceLoad);
    }

    /// <summary>
    /// 返回到上一个页面。
    /// </summary>
    public void Back()
    {
        if (Current == null || string.IsNullOrWhiteSpace(Current.BackUrl))
            return;

        NavigateTo(Current.BackUrl);
    }

    /// <summary>
    /// 导航到首页。
    /// </summary>
    /// <param name="returnUrl">登录返回地址。</param>
    /// <param name="forceLoad">是否强制刷新。</param>
    public void GoHomePage(string returnUrl = null, bool forceLoad = false)
    {
        var url = returnUrl ?? HomeUrl;
        NavigateTo(url, forceLoad);
    }

    /// <summary>
    /// 刷新当前页面。
    /// </summary>
    public void Refresh()
    {
        NavigateTo(Current?.RouteUrl, true);
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

        var menus = IsMobileApp ? [.. Config.AppMenus] : UserMenus;
        if (menus == null || menus.Count == 0)
            return;

        Current = GetCurrentMenu(menus, route);
    }

    private MenuInfo GetCurrentMenu(List<MenuInfo> menus, RouteData route)
    {
        if (menus == null || menus.Count == 0 || string.IsNullOrWhiteSpace(Url))
            return null;

        var url = Url;
        var index = url.IndexOf('?');
        if (index > 0)
            url = url[..index];
        if (route != null && route.RouteValues.Any() && route.PageType != typeof(AutoPage))
        {
            foreach (var item in route.RouteValues)
            {
                if (item.Value != null)
                    url = url.Replace($"{item.Value}", "").TrimEnd('/');
            }
        }
        var menu = menus.FirstOrDefault(m => m.Url == url || m.RouteUrl == url);
        if (menu != null)
            return menu;

        return menus.FirstOrDefault(m => m.HasRoute(url, route));
    }

    private void SetNavMenu(MenuInfo info)
    {
        if (string.IsNullOrWhiteSpace(info.BackUrl))
        {
            if (UserMenus?.Exists(m => m.Id == info.Id) == true)
                return;

            if (Config.AppMenus?.Any(m => m.Id == info.Id) == true)
                return;
        }

        var item = NavMenus.FirstOrDefault(m => m.Id == info.Id);
        if (item != null)
            NavMenus.Remove(item);

        NavMenus.Add(info);
    }
}