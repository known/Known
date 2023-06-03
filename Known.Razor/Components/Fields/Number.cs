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
            if (!string.IsNullOrWhiteSpace(Value))
                builder.Paragraph(attr => builder.Text($"{Value} {Unit}"));
            return;
        }

        builder.Input(attr =>
        {
            attr.Type("number").Id(Id).Name(Id).Placeholder(Placeholder).Value(Value)
                .Disabled(!Enabled).Required(Required).Readonly(ReadOnly).OnChange(CreateBinder());
        });
        if (!string.IsNullOrWhiteSpace(Unit))
            builder.Span("unit", Unit);
    }
}