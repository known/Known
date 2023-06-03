namespace Known.Razor.Components.Fields;

public class TextArea : Field
{
    [Parameter] public string Placeholder { get; set; }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        if (ReadOnly)
        {
            builder.Paragraph(attr => builder.Text(Value));
            return;
        }

        builder.TextArea(attr =>
        {
            attr.Id(Id).Name(Id).Value(Value).Disabled(!Enabled)
                .Placeholder(Placeholder)
                .OnChange(CreateBinder());
        });
    }
}