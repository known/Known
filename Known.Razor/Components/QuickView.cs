namespace Known.Razor.Components;

public class QuickView : BaseComponent
{
    [Parameter] public RenderFragment ChildContent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var css = CssBuilder.Default("quickview").Build();
        builder.Div(css, attr =>
        {
            attr.Id(Id);
            ChildContent?.Invoke(builder);
        });
    }
}