using Known.Blazor;
using Known.Extensions;

namespace Known;

public class Context
{
    private MenuItem current;

    internal static Action<MenuItem> OnNavigate { get; set; }
    internal static Action OnRefreshPage { get; set; }

    public IUIService UI { get; set; }
    public InstallInfo Install { get; internal set; }
    public UserInfo CurrentUser { get; internal set; }
    public SettingInfo UserSetting { get; internal set; }
    public List<MenuInfo> UserMenus { get; internal set; }
    public string CurrentLanguage { get; internal set; }

    public void SetCurrentLanguage(JSService service, string language)
    {
        CurrentLanguage = language;
        service.SetCurrentLanguage(language);
    }

    public void Back()
    {
        if (current == null || current.Previous == null)
            return;

        current = current.Previous;
        OnNavigate?.Invoke(current);
    }

    public List<MenuItem> GetMenus(List<string> menuIds)
    {
        if (menuIds == null || menuIds.Count == 0)
            return [];

        var menus = new List<MenuItem>();
        foreach (var menuId in menuIds)
        {
            var menu = UserMenus.FirstOrDefault(m => m.Name == menuId);
            if (menu != null)
                menus.Add(new MenuItem(menu));
        }
        return menus;
    }

    public void RefreshPage() => OnRefreshPage?.Invoke();

    public void NavigateToHome() => Navigate(Config.GetHomeMenu());
    public void NavigateToUserProfile() => Navigate(Config.GetUserProfileMenu());

    public void Navigate(MenuItem menu, Dictionary<string, object> prevParams = null)
    {
        if (menu == null)
            return;

        menu.Previous = current;
        if (menu.Previous != null && prevParams != null)
            menu.Previous.ComParameters = prevParams;
        current = menu;
        OnNavigate?.Invoke(current);
    }
}