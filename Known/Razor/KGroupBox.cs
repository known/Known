namespace Known.Razor;

public class KGroupBox : BaseComponent
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
            builder.Div("group-box-body", attr => builder.Fragment(ChildContent));
        });
    }
}