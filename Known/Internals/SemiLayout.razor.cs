namespace Known.Internals;

/// <summary>
/// Semi模板组件类。
/// </summary>
public partial class SemiLayout
{
    private KLayout layout;
    private MainBody body;
    //private MenuInfo root;
    private TreeModel tree;

    /// <summary>
    /// 取得或设置子组件内容。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    private string AppName => Config.App?.Name ?? "Known";
    private string SearchText { get; set; }
    private string UserName => CurrentUser?.Name ?? CurrentUser?.UserName ?? "Semi";
    private string UserRole => string.IsNullOrWhiteSpace(CurrentUser?.Role) ? "Administrator" : CurrentUser.Role;
    private string UserInitial => string.IsNullOrWhiteSpace(UserName) ? "S" : UserName[..1];

    private void OnLoadMenus(List<MenuInfo> menus)
    {
        //root = Config.App.GetRootMenu();
        //root.AddChildren(menus);

        tree = new TreeModel
        {
            ExpandRoot = true,
            Data = menus,
            SelectedKeys = Context.Current == null ? null : [Context.Current.Id],
            OnNodeClick = OnTreeClickAsync
        };
    }

    private void OnReloadPage()
    {
        body?.ReloadPage();
    }

    private void OnLogoClick()
    {
        Context.GoHomePage();
    }

    private Task OnTreeClickAsync(MenuInfo item)
    {
        if (item == null || string.IsNullOrWhiteSpace(item.RouteUrl))
            return Task.CompletedTask;

        Context.NavigateTo(item.RouteUrl);
        return Task.CompletedTask;
    }
}