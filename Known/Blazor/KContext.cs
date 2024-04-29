namespace Known.Blazor;

public class KContext : ComponentBase
{
    [Parameter] public Context Value { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Cascading(Value, ChildContent);
    }
}