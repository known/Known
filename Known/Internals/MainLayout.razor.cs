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

    private string LayoutClass => CssBuilder.Default("kui-layout").AddClass(Context.UserSetting.Size).BuildClass();
    private string HeaderClass => CssBuilder.Default("kui-header")
                                            .AddClass("kui-menu-dark", Context.UserSetting.MenuTheme == "Dark")
                                            .BuildClass();
    private string MenuClass => CssBuilder.Default()
                                          .AddClass("kui-menu-dark", Context.UserSetting.MenuTheme == "Dark")
                                          .AddClass("kui-menu-float", Context.UserSetting.LayoutMode == LayoutMode.Float.ToString())
                                          .BuildClass();

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
        menu?.SetData(root);
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