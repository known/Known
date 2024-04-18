namespace Known.Blazor;

public class KTitle : BaseComponent
{
    [Parameter] public string Text { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (!string.IsNullOrWhiteSpace(Text))
            builder.Div("kui-title", Text);
        else if (ChildContent != null)
            builder.Div("kui-title", () => ChildContent(builder));
    }
}