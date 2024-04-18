namespace Known.Blazor;

public class KToolbar : BaseComponent
{
    [Parameter] public RenderFragment ChildContent { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-toolbar", () => ChildContent(builder));
    }
}