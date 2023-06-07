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
               .Set(c => c.Style, $"{button.Style} {style}")
               .Set(c => c.Icon, button.Icon)
               .Set(c => c.Text, button.Name)
               .Set(c => c.OnClick, onClick)
               .Build();
    }

    public static void Button(this RenderTreeBuilder builder, string text, EventCallback onClick, string style = null) => builder.Button(text, "", onClick, style);

    public static void Button(this RenderTreeBuilder builder, string text, string icon, EventCallback onClick, string style = null)
    {
        builder.Component<Button>()
               .Set(c => c.Style, style)
               .Set(c => c.Icon, icon)
               .Set(c => c.Text, text)
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