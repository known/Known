namespace Known.Razor.Components;

public class GroupBox : BaseComponent
{
    [Parameter] public string Style { get; set; }
    [Parameter] public string Title { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div($"group-box {Style}", attr =>
        {
            builder.Label("legend", Title);
            builder.Fragment(ChildContent);
        });
    }
}