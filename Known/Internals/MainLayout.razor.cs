namespace Known.Internals;

/// <summary>
/// 主模板组件类。
/// </summary>
public partial class MainLayout
{
    private bool collapsed = false;

    private KLayout layout;
    private MainMenu menu;
    private MainBody body;
    private MenuInfo root;
    private MenuInfo topMenu;
    private readonly List<MenuInfo> topMenus = new();

    private UserSettingInfo UserSetting => Context.UserSetting ?? new();
    private MenuInfo MenuParent => Config.App.IsTopMenu ? topMenu ?? root : root;
    private string LayoutClass => CssBuilder.Default("kui-layout").AddClass(UserSetting.Size).BuildClass();
    private string HeaderClass => CssBuilder.Default("kui-header")
                                            .AddClass("kui-menu-dark", UserSetting.MenuTheme == "Dark")
                                            .BuildClass();
    private string SiderClass => CssBuilder.Default()
                                          .AddClass("kui-menu-dark", UserSetting.MenuTheme == "Dark")
                                          .AddClass("kui-menu-float", UserSetting.LayoutMode == LayoutMode.Float.ToString())
                                          .BuildClass();
    private string ScrollClass => CssBuilder.Default("kui-scroll").AddClass("is-trigger", Config.App.IsTopMenu).BuildClass();

    /// <summary>
    /// 取得或设置子组件内容。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <inheritdoc />
    protected override Task OnInitAsync()
    {
        collapsed = Context.IsMobile;
        return base.OnInitAsync();
    }

    private void OnLoadMenus(List<MenuInfo> menus)
    {
        root = Config.App.GetRootMenu();
        root.AddChildren(menus);

        topMenus.Clear();
        if (root?.Children != null && root.Children.Count > 0)
            topMenus.AddRange(root.Children);

        if (Config.App.IsTopMenu)
            topMenu = topMenus.FirstOrDefault() ?? root;

        menu?.SetData(MenuParent);
    }

    private void OnTopMenuClick(MenuInfo item)
    {
        if (item == null || item == topMenu)
            return;

        topMenu = item;
        menu?.SetData(topMenu);
        StateChanged();
    }

    private void OnReloadPage()
    {
        body?.ReloadPage();
    }

    private void OnToggle(bool collapsed)
    {
        this.collapsed = collapsed;
        StateChanged();
    }

    private void OnLogoClick()
    {
        Context.GoHomePage();
    }

    private void OnSetting()
    {
        layout?.ShowSetting();
    }
}