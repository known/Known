namespace Known.Razor;

class PageSingle : BaseComponent
{
    private DynamicComponent component;
    private KMenuItem curPage = Config.Home;

    internal void ShowPage(KMenuItem menu)
    {
        curPage = menu;
        StateChanged();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (curPage?.Name != Config.Home?.Name)
        {
            builder.Component<KBreadcrumb>().Set(c => c.Menu, curPage).Build();
            builder.Div("kui-content", attr =>
            {
                attr.AddRandomColor("border-top-color");
                BuildPage(builder);
            });
        }
        else
        {
            BuildPage(builder);
        }
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            UI.InitPage(curPage?.Id);

        return base.OnAfterRenderAsync(firstRender);
    }

    private void BuildPage(RenderTreeBuilder builder)
    {
        if (curPage == null || curPage.ComType == null)
            return;

        if (component != null)
            component = null;

        builder.DynamicComponent(curPage.ComType, curPage.ComParameters, value => component = value);
    }
}