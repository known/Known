namespace Known.Razor.Components.Fields;

public class Number : Input
{
    [Parameter] public string Unit { get; set; }

    protected override void BuildChildText(RenderTreeBuilder builder) => builder.Span("text", attr => BuildValueUnit(builder, Value, Unit));

    protected override void BuildChildContent(RenderTreeBuilder builder)
    {
        BuildInput(builder, "number");
        if (!string.IsNullOrWhiteSpace(Unit))
        {
            builder.Span("unit", Unit);
        }
    }

    private static void BuildValueUnit(RenderTreeBuilder builder, string value, string unit)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            builder.Text($"{value} {unit}");
        }
    }
}