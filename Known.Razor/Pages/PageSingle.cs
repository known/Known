namespace Known.Razor.Pages;

class PageSingle : BaseComponent
{
    [Parameter] public MenuItem CurPage { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (CurPage == null)
            return;

        if (!string.IsNullOrEmpty(CurPage.Description))
        {
            builder.Blockquote("", attr =>
            {
                builder.Text(CurPage.Name);
                builder.Small(CurPage.Description);
            });
        }

        if (CurPage.ComType != null)
        {
            builder.DynamicComponent(CurPage.ComType, CurPage.ComParameters);
        }
    }
}