namespace Known.Razor.Components.Fields;

public class Password : Field
{
    [Parameter] public string Icon { get; set; }
    [Parameter] public string Placeholder { get; set; }
    [Parameter] public string OnEnter { get; set; }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        builder.Label(attr =>
        {
            attr.For(Id);
            if (!string.IsNullOrWhiteSpace(Label))
                builder.Text(Label);
            BuildIcon(builder, Icon);
            builder.Input(attr =>
            {
                attr.Type("password").Id(Id).Name(Id).Placeholder(Placeholder).Value(Value)
                    .Disabled(!Enabled).Required(Required).Readonly(ReadOnly).OnChange(CreateBinder());
                AddError(attr);
            });
        });
    }

    protected override void BuildChildContent(RenderTreeBuilder builder)
    {
        BuildIcon(builder, Icon);
        builder.Input(attr =>
        {
            //var value = BindConverter.FormatValue(Value);
            //var hasChanged = !EqualityComparer<string>.Default.Equals(value, Value);
            attr.Type("password").Id(Id).Name(Id).Disabled(!Enabled)
                .Value(Value).Required(Required)
                .Placeholder(Placeholder)
                .Add("autocomplete", "off")
                .OnChange(CreateBinder())
                .OnEnter(OnEnter);
            //builder.SetUpdatesAttributeName("value");
        });
    }

    private static void BuildIcon(RenderTreeBuilder builder, string icon)
    {
        if (!string.IsNullOrWhiteSpace(icon))
            builder.Icon(icon);
    }
}

public class Text : Field
{
    [Parameter] public string Icon { get; set; }
    [Parameter] public string Placeholder { get; set; }
    [Parameter] public string OnEnter { get; set; }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        builder.Label(attr =>
        {
            attr.For(Id);
            if (!string.IsNullOrWhiteSpace(Label))
                builder.Text(Label);
            BuildIcon(builder, Icon);
            builder.Input(attr =>
            {
                attr.Type("text").Id(Id).Name(Id).Placeholder(Placeholder).Value(Value)
                    .Disabled(!Enabled).Required(Required).Readonly(ReadOnly).OnChange(CreateBinder());
                AddError(attr);
            });
        });
    }

    protected override void BuildChildContent(RenderTreeBuilder builder)
    {
        BuildIcon(builder, Icon);
        builder.Input(attr =>
        {
            //var value = BindConverter.FormatValue(Value);
            //var hasChanged = !EqualityComparer<string>.Default.Equals(value, Value);
            attr.Type("text").Id(Id).Name(Id).Disabled(!Enabled)
                .Value(Value).Required(Required)
                .Placeholder(Placeholder)
                .Add("autocomplete", "off")
                .OnChange(CreateBinder())
                .OnEnter(OnEnter);
            //builder.SetUpdatesAttributeName("value");
        });
    }

    private static void BuildIcon(RenderTreeBuilder builder, string icon)
    {
        if (!string.IsNullOrWhiteSpace(icon))
            builder.Icon(icon);
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