namespace Known.Razor.Components;

class PageSingle : BaseComponent
{
    [Parameter] public MenuItem CurPage { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (CurPage.Name != KRConfig.Home?.Name)
        {
            builder.Component<Breadcrumb>().Set(c => c.Menu, CurPage).Build();
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
        if (CurPage == null || CurPage.ComType == null)
            return;

        builder.DynamicComponent(CurPage.ComType, CurPage.ComParameters);
    }
}