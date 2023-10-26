using Known.Extensions;

namespace Known.Razor;

public class KProgress : BaseComponent
{
    [Parameter] public StyleType Style { get; set; }
    [Parameter] public decimal Value { get; set; }
    [Parameter] public int? Width { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var css = CssBuilder.Default("value").AddClass(Style.ToString().ToLower(), Value > 0).Build();
        builder.Div("progress", attr =>
        {
            if (Width != null && Width.Value > 0)
                attr.Style($"width:{Width}px;");
            builder.Span(css, attr =>
            {
                attr.Style($"width:{Value * 100}%;");
                builder.Text(Value.ToString("P0"));
            });
        });
    }
}