namespace Known.Razor.Components.Fields;

public class Password : Input
{
    [Parameter] public string Icon { get; set; }
    [Parameter] public string Placeholder { get; set; }

    protected override void BuildChildContent(RenderTreeBuilder builder)
    {
        BuildIcon(builder, Icon);
        BuildInput(builder, "password", Placeholder);
    }
}

public class Text : Input
{
    [Parameter] public string Icon { get; set; }
    [Parameter] public string Placeholder { get; set; }

    protected override void BuildChildContent(RenderTreeBuilder builder)
    {
        BuildIcon(builder, Icon);
        BuildInput(builder, "text", Placeholder);
    }
}

public class TextAction : Text
{
    [Parameter] public Action<string> OnOK { get; set; }

    protected override void BuildChildContent(RenderTreeBuilder builder)
    {
        BuildInput(builder, "text", Placeholder);
        builder.Span("btn", "确定", Callback(e => OnOK(Value)));
    }
}

public class TextArea : Field
{
    [Parameter] public string Placeholder { get; set; }

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