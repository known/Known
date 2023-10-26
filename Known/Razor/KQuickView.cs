using Known.Extensions;

namespace Known.Razor;

public class KQuickView : BaseComponent
{
    [Parameter] public string Style { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var css = CssBuilder.Default("quickview").AddClass(Style).Build();
        builder.Div(css, attr =>
        {
            attr.Id(Id);
            ChildContent?.Invoke(builder);
        });
    }
}