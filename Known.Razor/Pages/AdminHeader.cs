namespace Known.Razor.Pages;

class AdminHeader : BaseComponent
{
    private bool isMini = false;
    private bool isFull = false;
    private MenuItem curMenu;
    private string ToggleScreen => isFull ? "fa fa-arrows" : "fa fa-arrows-alt";
    private string IsActive(MenuItem menu) => curMenu?.Id == menu.Id ? "active" : "";

    [Parameter] public string AppName { get; set; }
    [Parameter] public int? MessageCount { get; set; }
    [Parameter] public List<MenuItem> Menus { get; set; }
    [Parameter] public Action<MenuItem> OnMenuClick { get; set; }
    [Parameter] public Action<bool> OnToggle { get; set; }
    [Parameter] public Action OnLogout { get; set; }

    protected override void OnInitialized()
    {
        if (Menus != null && Menus.Count > 0)
            curMenu = Menus.FirstOrDefault();

        PageAction.RefreshMessageCount = count =>
        {
            MessageCount = count;
            StateChanged();
        };
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Nav(attr =>
        {
            BuildNavLeft(builder);
            BuildNavRight(builder);
        });
    }

    private void BuildNavLeft(RenderTreeBuilder builder)
    {
        builder.Ul(attr =>
        {
            builder.Li("fa fa-bars", attr => attr.Title("折叠/展开").OnClick(Callback(OnToggleMenu)));
            builder.Li("name", attr => builder.Text(AppName));
            if (Menus != null && Menus.Count > 0)
            {
                foreach (var item in Menus)
                {
                    var active = IsActive(item);
                    builder.Li(active, attr =>
                    {
                        attr.OnClick(Callback(() => OnTopMenuClick(item)));
                        builder.IconName(item.Icon, item.Name, "name");
                    });
                }
            }
        });
    }

    private void BuildNavRight(RenderTreeBuilder builder)
    {
        builder.Ul(attr =>
        {
            //builder.Li("text danger", attr => builder.Text(KRConfig.AuthStatus));
            //builder.Li("text", attr => builder.Component<Components.Timer>().Build());
            //builder.Li("text", attr => builder.Text($"{DateTime.Now:yyyy-MM-dd dddd}"));
            builder.Li("fa fa-home", attr => attr.Title("系统主页").OnClick(Callback(Context.NavigateToHome)));
            if (KRConfig.IsWeb)
                builder.Li($"{ToggleScreen}", attr => attr.Title("全屏切换").OnClick(Callback(OnToggleScreen)));
            //builder.Li("fa fa-refresh", attr => attr.Title("刷新页面").OnClick(Callback(OnPageRefresh)));
            builder.Li("fa fa-user", attr =>
            {
                attr.Title("个人中心").OnClick(Callback(Context.NavigateToAccount));
                builder.Text(CurrentUser.Name);
                if (MessageCount > 0)
                    builder.Span("badge", $"{MessageCount}");
            });
            builder.Li("fa fa-power-off", attr => attr.Title("安全退出").OnClick(Callback(OnUserLogout)));
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