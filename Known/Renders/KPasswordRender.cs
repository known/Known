namespace Known.Renders;

class KPasswordRender : BaseRender<KPassword>
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        BuildIcon(builder, Component.Icon);
        builder.Input(attr =>
        {
            //var value = BindConverter.FormatValue(Value);
            //var hasChanged = !EqualityComparer<string>.Default.Equals(value, Value);
            attr.Type("password").Id(Component.Id).Name(Component.Id).Disabled(!Component.Enabled)
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