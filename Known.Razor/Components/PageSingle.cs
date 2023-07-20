namespace Known.Razor.Components;

class PageSingle : BaseComponent
{
    private MenuItem curPage = KRConfig.Home;

    internal void ShowPage(MenuItem menu)
    {
        curPage = menu;
        StateChanged();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (curPage?.Name != KRConfig.Home?.Name)
        {
            builder.Component<Breadcrumb>().Set(c => c.Menu, curPage).Build();
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

    private void BuildPage(RenderTreeBuilder builder)
    {
        if (curPage == null || curPage.ComType == null)
            return;

        builder.DynamicComponent(curPage.ComType, curPage.ComParameters);
    }
}