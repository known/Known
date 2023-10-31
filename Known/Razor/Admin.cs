namespace Known.Razor;

public class Admin : KLayout
{
    internal const string SysSettingId = "qvSysSetting";
    private bool isInitialized;
    private KMenuItem topMenu;
    private KMenuItem curMenu;
    private AdminInfo info;
    private List<KMenuItem> userMenus;

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
        info = await Platform.GetAdminAsync();
        Setting.UserSetting = info?.UserSetting;
        Setting.Info = info?.UserSetting?.Info ?? SettingInfo.Default;

        userMenus = GetUserMenus(info?.UserMenus);
        if (Config.IsWebApi)
            Cache.AttachCodes(info?.Codes);

        if (TopMenu)
        {
            topMenu = userMenus.FirstOrDefault();
            curMenu = topMenu.Children.FirstOrDefault();
        }

        PageAction.RefreshTheme = () =>
        {
            UI.CurDialog = UI.TopDialog;
            StateChanged();
        };
        isInitialized = true;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!isInitialized)
            return;

        builder.Cascading(this, b => base.BuildRenderTree(b));
        builder.Component<KQuickView>()
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

    private void OnMenuClick(KMenuItem menu)
    {
        topMenu = menu;
        curMenu = menu.Children.FirstOrDefault();
        StateChanged();
    }

    private void OnToggleSide(bool isMini) => UI.ToggleClass("app", "kui-mini");

    private List<KMenuItem> GetUserMenus(List<MenuInfo> menus)
    {
        Context.UserMenus = menus;
        return menus.ToMenuItems();
    }
}