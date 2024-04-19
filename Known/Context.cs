namespace Known;

public class Context
{
    private Language language;
    private string currentLanguage;

    public Context(IUIService ui)
    {
        UI = ui;
        LogoUrl = Theme == "dark" ? "img/logo.png" : "img/logo1.png";
    }

    internal Context(string cultureName)
    {
        language = new Language(cultureName);
    }

    internal MenuItem Current { get; set; }
    internal SysModule Module { get; set; }

    public IUIService UI { get; }
    public string Host { get; set; }
    public string Theme { get; set; }
    public string LogoUrl { get; set; }
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
}