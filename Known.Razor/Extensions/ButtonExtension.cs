namespace Known.Razor.Extensions;

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

        builder.Component<Button>()
               .Set(c => c.Type, button.Type)
               .Set(c => c.Icon, button.Icon)
               .Set(c => c.Text, button.Name)
               .Set(c => c.Style, style)
               .Set(c => c.OnClick, onClick)
               .Build();
    }

    public static void Button(this RenderTreeBuilder builder, string text, EventCallback onClick, StyleType type = StyleType.Default) => builder.Button(text, "", onClick, type);

    public static void Button(this RenderTreeBuilder builder, string text, string icon, EventCallback onClick, StyleType type = StyleType.Default, bool enabled = true)
    {
        builder.Component<Button>()
               .Set(c => c.Type, type)
               .Set(c => c.Icon, icon)
               .Set(c => c.Text, text)
               .Set(c => c.Enabled, enabled)
               .Set(c => c.OnClick, onClick)
               .Build();
    }

    internal static bool IsInMenu(this ButtonInfo button, string id)
    {
        var menu = KRConfig.UserMenus.FirstOrDefault(m => m.Id == id || m.Code == id);
        if (menu == null)
            return false;

        var hasButton = false;
        if (menu.Buttons != null && menu.Buttons.Count > 0)
            hasButton = menu.Buttons.Contains(button.Name);
        else if (menu.Actions != null && menu.Actions.Count > 0)
            hasButton = menu.Actions.Contains(button.Name);
        return hasButton;
    }
}