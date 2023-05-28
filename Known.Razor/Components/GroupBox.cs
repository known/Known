namespace Known.Razor.Components;

public class GroupBox : BaseComponent
{
    [Parameter] public string Style { get; set; }
    [Parameter] public string Title { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var css = CssBuilder.Default("group-box").AddClass(Style).Build();
        builder.Div(css, attr =>
        {
            builder.Label("legend", Title);
            builder.Fragment(ChildContent);
        });
    }
}