namespace Known.Razor.Components.Fields;

public class Number : Field
{
    [Parameter] public string Unit { get; set; }

    [Parameter] public string Type { get; set; }
    [Parameter] public string Placeholder { get; set; }
    [Parameter] public string OnEnter { get; set; }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        if (ReadOnly)
        {
            builder.Span(Value);
            return;
        }

        builder.Label(attr =>
        {
            attr.For(Id);
            if (!string.IsNullOrWhiteSpace(Label))
                builder.Text(Label);
            builder.Input(attr =>
            {
                attr.Type(Type).Id(Id).Name(Id).Placeholder(Placeholder).Value(Value)
                    .Disabled(!Enabled).Required(Required).Readonly(ReadOnly).OnChange(CreateBinder());
            });
        });
    }

    protected override void BuildChildContent(RenderTreeBuilder builder)
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

    protected override void BuildChildText(RenderTreeBuilder builder) => builder.Span("text", attr => BuildValueUnit(builder, Value, Unit));

    private static void BuildValueUnit(RenderTreeBuilder builder, string value, string unit)
    {
        if (!string.IsNullOrWhiteSpace(value))
            builder.Text($"{value} {unit}");
    }
}