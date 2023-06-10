namespace Known.Razor.Pages;

class PageSingle : BaseComponent
{
    private bool HasModuleTips => ShowTips && !string.IsNullOrWhiteSpace(CurPage?.Description);

    [Parameter] public bool ShowTips { get; set; }
    [Parameter] public MenuItem CurPage { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (HasModuleTips)
            BuildModuleTips(builder);
        BuildContent(builder);
    }

    private void BuildModuleTips(RenderTreeBuilder builder)
    {
        builder.Div("kui-tips", attr =>
        {
            builder.Span("name", CurPage?.Name);
            builder.Span("description", CurPage?.Description);
        });
    }

    private void BuildContent(RenderTreeBuilder builder)
    {
        if (HasModuleTips)
        {
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