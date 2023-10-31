namespace Known.Extensions;

public static class ButtonExtension
{
    public static void Button(this RenderTreeBuilder builder, string className, Action<AttributeBuilder> child = null)
    {
        builder.Element("button", attr =>
        {
            attr.Class(className);
            child?.Invoke(attr);
        });
    }

    public static void Button(this RenderTreeBuilder builder, ButtonInfo button, EventCallback onClick, bool visible = true, string style = null)
    {
        if (!visible)
            return;

        builder.Component<KButton>()
               .Set(c => c.Type, button.Type)
               .Set(c => c.Icon, button.Icon)
               .Set(c => c.Text, button.Name)
               .Set(c => c.Enabled, button.Enabled)
               .Set(c => c.Style, style)
               .Set(c => c.OnClick, onClick)
               .Build();
    }

    public static void Button(this RenderTreeBuilder builder, string text, EventCallback onClick, StyleType type = StyleType.Default, bool enabled = true) => builder.Button(text, "", onClick, type, enabled);

    public static void Button(this RenderTreeBuilder builder, string text, string icon, EventCallback onClick, StyleType type = StyleType.Default, bool enabled = true)
    {
        builder.Component<KButton>()
               .Set(c => c.Type, type)
               .Set(c => c.Icon, icon)
               .Set(c => c.Text, text)
               .Set(c => c.Enabled, enabled)
               .Set(c => c.OnClick, onClick)
               .Build();
    }
}