namespace Known.Razor.Pages;

public class AdminFlat : BaseComponent
{
    private bool isInitialized;
    private bool isHome = true;
    private bool hasWifi = false;
    private MenuItem curMenu;
    private MenuItem curSubMenu;
    private Type currType;
    private AdminInfo info;

    public AdminFlat()
    {
        isInitialized = false;
        KRContext.OnNavigate = info =>
        {
            Navigate(info);
            StateHasChanged();
        };
    }

    [Parameter] public Action OnLogout { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (CurrentUser != null)
            OnClickMenu();

        info = await Platform.User.GetAdminAsync();
        hasWifi = Utils.HasNetwork();
        isInitialized = true;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!isInitialized)
            return;

        if (CurrentUser == null)
        {
            builder.Markup("<div class=\"startup\">系统启动中......</div>");
        }
        else
        {
            builder.Div("app", attr =>
            {
                BuildSider(builder);
                BuildHeader(builder);
                BuildMain(builder);
            });
        }
    }

    private void BuildSider(RenderTreeBuilder builder)
    {
        builder.Div("sider", attr =>
        {
            builder.Div("fa fa-university logo", attr =>
            {
                attr.OnClick(Callback(e => OnClickMenu()));
            });
            builder.Component<Menu>()
                   .Set(c => c.Style, "menu menu1")
                   .Set(c => c.OnlyIcon, true)
                   .Set(c => c.CurItem, curMenu)
                   //.Set(c => c.Items, info?.UserMenus?.ToArray())
                   .Set(c => c.OnChanged, Callback<MenuItem>(OnClickMenu))
                   .Build();
        });
    }

    private void BuildHeader(RenderTreeBuilder builder)
    {
        builder.Div("header", attr =>
        {
            builder.Div("title welcome", attr => builder.Text(info?.AppName));
            if (hasWifi)
            {
                builder.Icon("fa fa-wifi wifi");
            }
            builder.Div("time", attr => builder.Component<Components.Timer>().Build());
            builder.Div("lock fa fa-power-off", attr =>
            {
                attr.OnClick(Callback(e => OnLock()));
            });
        });
    }

    private void BuildMain(RenderTreeBuilder builder)
    {
        builder.Div("main", attr =>
        {
            if (isHome)
            {
                //new Dictionary<string, object>
                //{
                //    ["OnMenuClick"] = Callback<MenuInfo>(e => OnClickMenu(e))
                //});
                builder.DynamicComponent(KRConfig.Home.ComType);
            }
            else
            {
                BuildSubMenu(builder);
                builder.Div("content", attr => builder.DynamicComponent(currType));
            }
        });
    }

    private void BuildSubMenu(RenderTreeBuilder builder)
    {
        builder.Div("title module", attr => builder.Text(curMenu?.Name));
        builder.Component<Menu>()
               .Set(c => c.Style, "menu3")
               .Set(c => c.CurItem, curSubMenu)
               .Set(c => c.Items, curMenu?.Children.ToArray())
               .Set(c => c.OnChanged, Callback<MenuItem>(OnClickSubMenu))
               .Build();
    }

    private void OnClickMenu(MenuItem menu = null)
    {
        isHome = menu == null;
        curMenu = menu;
        if (menu != null)
        {
            if (menu.Children != null && menu.Children.Count > 0)
                OnClickSubMenu((MenuItem)menu.Children[0]);
            else
                currType = menu.ComType;
        }
    }

    private void OnClickSubMenu(MenuItem menu)
    {
        curSubMenu = menu;
        currType = menu.ComType;
    }

    private async void OnLock()
    {
        var user = CurrentUser;
        if (user != null)
        {
            await Platform.User.SignOutAsync(user.Token);
            OnLogout?.Invoke();
        }
    }

    private void Navigate(MenuItem info) => currType = info.ComType;
}