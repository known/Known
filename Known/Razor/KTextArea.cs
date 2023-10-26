namespace Known.Razor;

public class KTextArea : Field
{
    [Parameter] public string Placeholder { get; set; }

    protected override void BuildText(RenderTreeBuilder builder)
    {
        builder.Pre(Value);
    }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        builder.TextArea(attr =>
        {
            attr.Id(Id).Name(Id).Value(Value).Disabled(!Enabled)
                .Placeholder(Placeholder)
                .OnChange(CreateBinder());
        });
    }
}