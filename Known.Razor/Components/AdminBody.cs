namespace Known.Razor.Components;

public class AdminBody : BaseComponent
{
    private PageTabs tabs;
    private PageSingle page;

    [Parameter] public bool MultiTab { get; set; }

    protected override void OnInitialized()
    {
        KRContext.OnNavigate = OnNavigate;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (MultiTab)
            builder.Component<PageTabs>().Build(value => tabs = value);
        else
            builder.Component<PageSingle>().Build(value => page = value);
    }

    private void OnNavigate(MenuItem menu)
    {
        menu.ComType = KRConfig.GetType(menu.Target);
        if (menu == null || menu.ComType == null)
            return;

        if (MultiTab)
        {
            tabs?.ShowTab(menu);
            return;
        }

        page?.ShowPage(menu);
    }
}