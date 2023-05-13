namespace Known.Razor.Apps;

public class AppMenuItem : MenuItem
{
    public bool HideTopbar { get; set; }
    public Action Action { get; set; }
}

public class Mobile : BaseComponent
{
    private int top;
    private int bottom;
    private bool showBack;
    private bool showTopbar;
    private bool showTabbar;
    private AppMenuItem curMenu;
    private Type curType;
    private Dictionary<string, object> curParameters;

    [Parameter] public AppMenuItem[] TabMenus { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await Platform.Dictionary.RefreshCache();
        KRContext.OnNavigate = menu => OnNavigate((AppMenuItem)menu);
        if (TabMenus != null && TabMenus.Length > 0)
        {
            Context.Navigate(TabMenus[0]);
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("app-mobile", attr =>
        {
            BuildTopBar(builder);
            BuildBody(builder);
            BuildTabbar(builder);
        });
        builder.Component<DialogContainer>().Build();
    }

    private void BuildTopBar(RenderTreeBuilder builder)
    {
        if (!showTopbar)
            return;

        builder.Component<Topbar>()
               .Set(c => c.ShowBack, showBack)
               .Set(c => c.Title, curMenu.Name)
               .Build();
    }

    private void BuildBody(RenderTreeBuilder builder)
    {
        builder.Div("content", attr =>
        {
            attr.Style($"top:{top}px;bottom:{bottom}px;");
            builder.DynamicComponent(curType, curParameters);
        });
    }

    private void BuildTabbar(RenderTreeBuilder builder)
    {
        if (!showTabbar)
            return;

        builder.Component<Tabbar>()
               .Set(c => c.Items, TabMenus)
               .Set(c => c.CurItem, curMenu)
               .Set(c => c.OnChanged, Callback<MenuItem>(OnMenuChanged))
               .Build();
    }

    private void OnMenuChanged(MenuItem menu) => Context.Navigate(menu);

    private void OnNavigate(AppMenuItem menu)
    {
        curMenu = menu;
        curType = menu.ComType;
        curParameters = menu.ComParameters;
        showBack = !TabMenus.Contains(menu);
        showTopbar = !menu.HideTopbar;
        showTabbar = true;// !showBack;
        top = showTopbar ? 50 : 0;
        bottom = showTabbar ? 51 : 0;
        StateChanged();
    }
}