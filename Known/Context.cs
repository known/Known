﻿namespace Known;

public class Context
{
    private Language language;
    private string currentLanguage;

    public Context() { }

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
    public string Url { get; set; }
    public string Theme { get; set; }
    public string LogoUrl => Theme == "dark" ? "img/logo.png" : "img/logo1.png";
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

    public bool HasButton(string buttonId)
    {
        var user = CurrentUser;
        if (user == null)
            return false;

        if (user.IsAdmin)
            return true;

        return IsInMenu(Module?.Id, buttonId);
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