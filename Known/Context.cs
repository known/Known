namespace Known;

public class Context
{
    private Language language;
    private string currentLanguage;

    public Context(IUIService ui)
    {
        UI = ui;
    }

    internal Context(string cultureName)
    {
        language = new Language(cultureName);
    }

    internal SysModule Module { get; set; }
    public MenuInfo Current { get; private set; }
    public IUIService UI { get; }
    public bool IsMobile { get; set; }
    public string Host { get; set; }
    public string Url { get; set; }
    public string Theme { get; set; }
    public string LogoUrl => Theme == "dark" ? "img/logo.png" : "img/logo1.png";
    public InstallInfo Install { get; internal set; }
    public UserInfo CurrentUser { get; internal set; }
    public SettingInfo UserSetting { get; internal set; } = new();
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

    internal async Task SetCurrentMenuAsync(PlatformService platform, string pageId = "")
    {
        Module = null;
        Current = Config.Menus.FirstOrDefault(m => m.Url == Url || m.Id == pageId);
        if (Current == null)
        {
            var menus = IsMobile ? Config.AppMenus : UserMenus;
            Current = menus?.FirstOrDefault(m => m.Url == Url || m.Id == pageId);
            if (Current != null)
                Module = await platform.Module.GetModuleAsync(Current.Id);
        }

        if (Module == null)
            return;

        var log = new SysLog
        {
            Target = Module.Name,
            Content = Url,
            Type = LogType.Page.ToString()
        };
        await platform.System.AddLogAsync(log);
    }
}