namespace Known.Extensions;

public static class ElementExtension
{
    public static void Element(this RenderTreeBuilder builder, string name, Action<AttributeBuilder> child = null)
    {
        builder.OpenElement(0, name);
        var attr = new AttributeBuilder(builder);
        child?.Invoke(attr);
        builder.CloseElement();
    }

    public static void Fragment(this RenderTreeBuilder builder, RenderFragment fragment)
    {
        if (fragment != null)
            builder.AddContent(1, fragment);
    }

    public static void Fragment<TValue>(this RenderTreeBuilder builder, RenderFragment<TValue> fragment, TValue value)
    {
        if (fragment != null)
            builder.AddContent(1, fragment, value);
    }

    public static void Markup(this RenderTreeBuilder builder, string markup)
    {
        if (!string.IsNullOrWhiteSpace(markup))
            builder.AddMarkupContent(1, markup);
    }

    public static void Text(this RenderTreeBuilder builder, string text)
    {
        if (!string.IsNullOrWhiteSpace(text))
            builder.AddContent(1, text);
    }

    public static void IconName(this RenderTreeBuilder builder, string icon, string name)
    {
        if (!string.IsNullOrWhiteSpace(icon))
            builder.Icon(icon);
        builder.Span(name);
    }

    internal static void IconName(this RenderTreeBuilder builder, string icon, string name, string nameStyle)
    {
        if (!string.IsNullOrWhiteSpace(icon))
            builder.Icon(icon);
        builder.Span(nameStyle, name);
    }
}