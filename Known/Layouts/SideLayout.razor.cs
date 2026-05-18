namespace Known.Layouts;

/// <summary>
/// 简洁边栏模板组件类。
/// </summary>
public partial class SideLayout
{
    private KLayout layout;
    private MainMenu menu;
    private MainBody body;
    private MenuInfo root;

    private UserSettingInfo UserSetting => Context.UserSetting ?? new();
    private string SiderClass => CssBuilder.Default("ks-side")
                                           .AddClass("menu-dark", UserSetting.MenuTheme == "Dark")
                                           .BuildClass();

    /// <summary>
    /// 取得或设置子组件内容。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

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

    private void OnLogoClick()
    {
        Context.GoHomePage();
    }

    private void OnSetting()
    {
        layout?.ShowSetting();
    }
}