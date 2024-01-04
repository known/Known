using System.Globalization;
using Known.Blazor;

namespace Known;

public class Context
{
    private MenuItem current;
    private Language language;
    private string currentLanguage;

    internal static Action<MenuItem> OnNavigate { get; set; }
    internal static Action OnRefreshPage { get; set; }

    public IUIService UI { get; set; }
    public InstallInfo Install { get; internal set; }
    public UserInfo CurrentUser { get; internal set; }
    public SettingInfo UserSetting { get; internal set; }
    public List<MenuInfo> UserMenus { get; internal set; }

    public string CurrentLanguage
    {
        get { return currentLanguage; }
        set
        {
            currentLanguage = value;
            language = new Language(value);
            UI.Language = language;
            var culture = new CultureInfo(language.Name);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }

    public Language Language
    {
        get
        {
            language ??= new Language(CurrentLanguage);
            return language;
        }
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