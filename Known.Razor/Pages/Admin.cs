namespace Known.Razor.Pages;

class Admin : Layout
{
    internal const string SysSettingId = "qvSysSetting";
    private bool isInitialized;
    private MenuItem topMenu;
    private MenuItem curMenu;
    private AdminInfo info;
    private List<MenuItem> userMenus;

    public Admin()
    {
        Style = "kui-layout";
        Header = "kui-header";
        Sider = "kui-sider";
        Body = "kui-body";
    }

    [Parameter] public bool TopMenu { get; set; }
    [Parameter] public Action OnLogout { get; set; }

    protected override async Task OnInitializedAsync()
    {
        info = await Platform.User.GetAdminAsync();
        Setting.UserSetting = info?.UserSetting;
        Setting.Info = info?.UserSetting?.Info ?? SettingInfo.Default;
        
        userMenus = GetUserMenus(info?.UserMenus);
        if (Context.IsWebApi)
            Cache.AttachCodes(info?.Codes);

        if (TopMenu)
        {
            topMenu = userMenus.FirstOrDefault();
            curMenu = topMenu.Children.FirstOrDefault();
        }

        PageAction.RefreshTheme = StateChanged;
        isInitialized = true;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!isInitialized)
            return;

        base.BuildRenderTree(builder);
        builder.Component<QuickView>()
               .Set(c => c.Id, SysSettingId)
               .Set(c => c.ChildContent, b => b.Component<SettingForm>().Set(s => s.Title, "系统设置").Build())
               .Build();
    }

    protected override void BuildHeader(RenderTreeBuilder builder)
    {
        builder.Component<AdminHeader>()
               .Set(c => c.AppName, info?.AppName)
               .Set(c => c.MessageCount, info?.MessageCount)
               .Set(c => c.Menus, TopMenu ? userMenus : null)
               .Set(c => c.OnMenuClick, OnMenuClick)
               .Set(c => c.OnToggle, OnToggleSide)
               .Set(c => c.OnLogout, OnLogout)
               .Build();
    }

    protected override void BuildSider(RenderTreeBuilder builder)
    {
        builder.Component<AdminSider>()
               .Set(c => c.CurMenu, curMenu)
               .Set(c => c.Menus, TopMenu ? topMenu?.Children : userMenus)
               .Build();
    }

    protected override void BuildBody(RenderTreeBuilder builder)
    {
        builder.Component<AdminBody>()
               .Set(c => c.MultiTab, Setting.Info.MultiTab)
               .Build();
    }

    private void OnMenuClick(MenuItem menu)
    {
        topMenu = menu;
        curMenu = menu.Children.FirstOrDefault();
        StateChanged();
    }

    private void OnToggleSide(bool isMini) => UI.ToggleClass("app", "kui-mini");

    private static List<MenuItem> GetUserMenus(List<MenuInfo> menus)
    {
        KRConfig.UserMenus = menus;
        return menus.ToMenuItems();
    }
}