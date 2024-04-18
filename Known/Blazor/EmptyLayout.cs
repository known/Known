namespace Known.Blazor;

public class EmptyLayout : LayoutComponentBase
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.AddContent(0, Body);
    }
}