namespace Known.Razor.Components.Fields;

public class Text : Field
{
    [Parameter] public string Icon { get; set; }
    [Parameter] public string Placeholder { get; set; }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        if (ReadOnly)
        {
            builder.Paragraph(attr => builder.Text(Value));
            return;
        }

        if (!string.IsNullOrWhiteSpace(Icon))
            builder.Icon(Icon);
        builder.Input(attr =>
        {
            attr.Type("text").Id(Id).Name(Id).Placeholder(Placeholder).Value(Value)
                .Disabled(!Enabled).Required(Required).Readonly(ReadOnly).OnChange(CreateBinder());
        });
    }
}