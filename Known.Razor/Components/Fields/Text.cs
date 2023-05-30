namespace Known.Razor.Components.Fields;

public class Password : Input
{
    public Password()
    {
        Type = "password";
    }

    [Parameter] public string Icon { get; set; }
    
    protected override void BuildChildContent(RenderTreeBuilder builder)
    {
        BuildIcon(builder, Icon);
        BuildInput(builder, "password", Placeholder);
    }
}

public class Text : Input
{
    public Text()
    {
        Type = "text";
    }

    [Parameter] public string Icon { get; set; }

    protected override void BuildChildContent(RenderTreeBuilder builder)
    {
        BuildIcon(builder, Icon);
        BuildInput(builder, "text", Placeholder);
    }
}

public class TextArea : Field
{
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