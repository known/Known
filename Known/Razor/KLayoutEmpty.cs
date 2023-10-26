namespace Known.Razor;

public class KLayoutEmpty : LayoutComponentBase
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.AddContent(0, base.Body);
    }
}