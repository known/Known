namespace Known.Renders;

class KInputRender : BaseRender<KInput>
{
    private string type => Component.Type.ToString().ToLower();

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var enabled = Component.Enabled;
        if (Component.Type == InputType.Color && Component.ReadOnly)
            enabled = false;

        BuildIcon(builder, Component.Icon);
        builder.Input(attr =>
        {
            //var value = BindConverter.FormatValue(Value);
            //var hasChanged = !EqualityComparer<string>.Default.Equals(value, Value);
            attr.Type(type).Id(Component.Id).Name(Component.Id).Disabled(!enabled).Readonly(Component.ReadOnly)
                .Value(Component.Value).Required(Component.Required)
                .Placeholder(Component.Placeholder)
                .Add("autocomplete", "off")
                .OnChange(Component.CreateBinder())
                .OnEnter(Component.OnEnter);
            //builder.SetUpdatesAttributeName("value");
        });
    }

    private static void BuildIcon(RenderTreeBuilder builder, string icon)
    {
        if (!string.IsNullOrWhiteSpace(icon))
            builder.Icon(icon);
    }
}