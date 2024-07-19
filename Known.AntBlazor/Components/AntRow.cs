namespace Known.AntBlazor.Components;

public class AntRow : ComponentBase
{
    [Parameter] public RenderFragment ChildContent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("ant-row", () => builder.Fragment(ChildContent));
    }
}