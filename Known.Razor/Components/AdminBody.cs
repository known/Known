namespace Known.Razor.Components;

public class AdminBody : BaseComponent
{
    private MenuItem curPage;
    private PageTabs tabs;

    [Parameter] public bool MultiTab { get; set; }

    protected override void OnInitialized()
    {
        KRContext.OnNavigate = OnNavigate;
        curPage = KRConfig.Home;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (MultiTab)
        {
            builder.Component<PageTabs>().Build(v => tabs = v);
        }
        else
        {
            builder.Component<PageSingle>()
                   .Set(c => c.CurPage, curPage)
                   .Build();
        }
    }

    private void OnNavigate(MenuItem menu)
    {
        menu.ComType = KRConfig.GetType(menu.Target);
        if (menu == null || menu.ComType == null)
            return;

        if (MultiTab)
        {
            tabs?.AddTab(menu);
            return;
        }

        curPage = menu;
        StateChanged();
    }
}