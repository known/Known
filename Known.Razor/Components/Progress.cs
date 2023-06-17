namespace Known.Razor.Components;

public class Progress : BaseComponent
{
    [Parameter] public int Width { get; set; }
    [Parameter] public decimal Value { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("progress", attr =>
        {
            if (Width > 0)
                attr.Style($"width:{Width}px;");
            builder.Span("value", attr =>
            {
                attr.Style($"width:{Value * 100}%;");
                builder.Text(Value.ToString("P0"));
            });
        });
    }
}