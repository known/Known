using Known.Extensions;

namespace Known.Razor;

public class KNumber : Field
{
    [Parameter] public string Unit { get; set; }

    [Parameter] public string Type { get; set; }
    [Parameter] public string Placeholder { get; set; }
    [Parameter] public string OnEnter { get; set; }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        builder.Input(attr =>
        {
            //var value = BindConverter.FormatValue(Value);
            //var hasChanged = !EqualityComparer<string>.Default.Equals(value, Value);
            attr.Type("number").Id(Id).Name(Id).Disabled(!Enabled)
                .Value(Value).Required(Required)
                .Placeholder(Placeholder)
                .Add("autocomplete", "off")
                .OnChange(CreateBinder())
                .OnEnter(OnEnter);
            //builder.SetUpdatesAttributeName("value");
        });
        if (!string.IsNullOrWhiteSpace(Unit))
            builder.Span("unit", Unit);
    }

    protected override void BuildText(RenderTreeBuilder builder) => builder.Span("text", attr => BuildValueUnit(builder, Value, Unit));

    private static void BuildValueUnit(RenderTreeBuilder builder, string value, string unit)
    {
        if (!string.IsNullOrWhiteSpace(value))
            builder.Text($"{value} {unit}");
    }
}