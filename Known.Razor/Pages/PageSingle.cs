namespace Known.Razor.Pages;

class PageSingle : BaseComponent
{
    private bool HasModuleTips => ShowTips && !string.IsNullOrWhiteSpace(CurPage?.Description);

    [Parameter] public bool ShowTips { get; set; }
    [Parameter] public MenuItem CurPage { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (HasModuleTips)
        {
            BuildModuleTips(builder);
        }
        BuildContent(builder);
    }

    private void BuildModuleTips(RenderTreeBuilder builder)
    {
        builder.Div("kui-mod-tips box", attr =>
        {
            builder.Span("name", CurPage?.Name);
            builder.Span("description", CurPage?.Description);
        });
    }

    private void BuildContent(RenderTreeBuilder builder)
    {
        builder.Div(HasModuleTips ? "kui-content1" : "kui-content", attr =>
        {
            if (CurPage != null && CurPage.ComType != null)
            {
                builder.DynamicComponent(CurPage.ComType, CurPage.ComParameters);
            }
        });
    }
}