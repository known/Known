namespace Known.Razor.Pages;

class Admin : BaseComponent
{
    private bool isInitialized;
    private bool isMultiTab;
    private MenuItem topMenu;
    private MenuItem curMenu;
    private AdminInfo info;
    private List<MenuItem> userMenus;

    [Parameter] public bool TopMenu { get; set; }
    [Parameter] public Action OnLogout { get; set; }

    protected override async Task OnInitializedAsync()
    {
        info = await Platform.User.GetAdminAsync();
        Setting.UserSetting = info.UserSetting;
        isMultiTab = Setting.Info.MultiTab;
        
        userMenus = GetUserMenus(info.UserMenus);
        if (Context.IsWebApi)
            Cache.AttachCodes(info.Codes);

        if (TopMenu)
        {
            topMenu = userMenus.FirstOrDefault();
            curMenu = topMenu.Children.FirstOrDefault();
        }

        isInitialized = true;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!isInitialized)
            return;

        BuildHeader(builder);
        BuildSider(builder);
        BuildBody(builder);
    }

    private void BuildHeader(RenderTreeBuilder builder)
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

    private void BuildSider(RenderTreeBuilder builder)
    {
        builder.Component<AdminSider>()
               .Set(c => c.CurMenu, curMenu)
               .Set(c => c.Menus, TopMenu ? topMenu?.Children : userMenus)
               .Build();
    }

    private void BuildBody(RenderTreeBuilder builder)
    {
        builder.Component<AdminBody>()
               .Set(c => c.MultiTab, isMultiTab)
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