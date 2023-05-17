namespace Known.Razor.Pages;

class AdminHeader : BaseComponent
{
    private bool isMini = false;
    private bool isFull = false;
    private MenuItem curMenu;
    private string ToggleIcon => isMini ? "fa fa-indent" : "fa fa-dedent";
    private string ToggleScreen => isFull ? "fa fa-arrows" : "fa fa-arrows-alt";
    private string IsActive(MenuItem menu) => curMenu?.Id == menu.Id ? "active" : "";

    [Parameter] public string AppName { get; set; }
    [Parameter] public List<MenuItem> Menus { get; set; }
    [Parameter] public Action<MenuItem> OnMenuClick { get; set; }
    [Parameter] public Action<bool> OnToggle { get; set; }
    [Parameter] public Action OnLogout { get; set; }

    protected override void OnInitialized()
    {
        if (Menus != null && Menus.Count > 0)
            curMenu = Menus.FirstOrDefault();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("kui-header", attr =>
        {
            if (Menus != null && Menus.Count > 0)
                BuildMenus(builder, Menus);
            else
                BuildAppName(builder);
            BuildNavRight(builder);
        });
    }

    private void BuildMenus(RenderTreeBuilder builder, List<MenuItem> menus)
    {
        builder.Ul("topMenu", attr =>
        {
            foreach (var item in menus)
            {
                var active = IsActive(item);
                builder.Li(active, attr =>
                {
                    attr.OnClick(Callback(() => OnTopMenuClick(item)));
                    builder.IconName(item.Icon, item.Name, "name");
                });
            }
        });
    }

    private void BuildAppName(RenderTreeBuilder builder)
    {
        builder.Div("toggleMenu", attr =>
        {
            builder.Icon(ToggleIcon, attr => attr.Title("折叠/展开").OnClick(Callback(OnToggleMenu)));
        });
        builder.Div("appName", AppName);
    }

    private void BuildNavRight(RenderTreeBuilder builder)
    {
        builder.Ul("nav right", attr =>
        {
            builder.Li("nav-item text danger", attr => builder.Text(KRConfig.AuthStatus));
            //builder.Li("nav-item text", attr => builder.Component<Components.Timer>().Build());
            builder.Li("nav-item text", attr => builder.Text($"{DateTime.Now:yyyy-MM-dd dddd}"));
            builder.Li("nav-item fa fa-home", attr => attr.Title("系统主页").OnClick(Callback(Context.NavigateToHome)));
            if (KRConfig.IsWeb)
                builder.Li($"nav-item {ToggleScreen}", attr => attr.Title("全屏切换").OnClick(Callback(OnToggleScreen)));
            //builder.Li("nav-item fa fa-refresh", attr => attr.Title("刷新页面").OnClick(Callback(OnPageRefresh)));
            builder.Li("nav-item fa fa-user", attr => attr.Title("个人中心").OnClick(Callback(Context.NavigateToAccount)));
            builder.Li("nav-item fa fa-power-off", attr => attr.Title("安全退出").OnClick(Callback(OnUserLogout)));
        });
    }

    private void OnTopMenuClick(MenuItem item)
    {
        curMenu = item;
        OnMenuClick?.Invoke(item);
    }

    private void OnToggleMenu()
    {
        isMini = !isMini;
        OnToggle?.Invoke(isMini);
    }

    private void OnToggleScreen()
    {
        isFull = !isFull;
        if (isFull)
            UI.OpenFullScreen();
        else
            UI.CloseFullScreen();
    }

    private void OnUserLogout()
    {
        UI.Confirm("确定要退出系统？", async () =>
        {
            var user = CurrentUser;
            if (user != null)
            {
                await Platform.User.SignOutAsync(user.Token);
                OnLogout?.Invoke();
            }
        }, true);
    }
}