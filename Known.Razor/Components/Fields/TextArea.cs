namespace Known.Razor.Components.Fields;

public class TextArea : Field
{
    [Parameter] public string Placeholder { get; set; }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        if (ReadOnly)
        {
            builder.Span(Value);
            return;
        }

        builder.TextArea(attr =>
        {
            attr.Id(Id).Name(Id).Value(Value).Disabled(!Enabled)
                .Placeholder(Placeholder)
                .OnChange(CreateBinder());
        });
    }

    protected override void BuildChildText(RenderTreeBuilder builder)
    {
        builder.Pre(Value);
    }

    protected override void BuildChildContent(RenderTreeBuilder builder)
    {
        builder.TextArea(attr =>
        {
            attr.Id(Id).Name(Id).Value(Value).Disabled(!Enabled)
                .Placeholder(Placeholder)
                .OnChange(CreateBinder());
        });
    }
}