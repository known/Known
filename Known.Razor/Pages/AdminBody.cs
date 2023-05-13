namespace Known.Razor.Pages;

public class AdminBody : BaseComponent
{
    private readonly List<MenuItem> menus = new();
    private MenuItem curPage;

    [Parameter] public bool MultiTab { get; set; }

    protected override void OnInitialized()
    {
        KRContext.OnNavigate = OnNavigate;
        curPage = KRConfig.Home;
        if (MultiTab && curPage != null)
            menus.Add(curPage);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("kui-body", attr =>
        {
            if (MultiTab)
            {
                builder.Component<PageTabs>()
                       .Set(c => c.CurPage, curPage)
                       .Set(c => c.Menus, menus)
                       .Build();
            }
            else
            {
                builder.Component<PageSingle>()
                       .Set(c => c.CurPage, curPage)
                       .Set(c => c.ShowTips, true)
                       .Build();
            }
        });
    }

    private void OnNavigate(MenuItem menu)
    {
        menu.ComType = KRConfig.GetType(menu.Target);
        if (menu == null || menu.ComType == null)
            return;

        curPage = menu;
        if (MultiTab)
            UI.PageId = menu.Id;
        if (MultiTab && !menus.Contains(menu))
            menus.Add(menu);
        StateChanged();
    }
}