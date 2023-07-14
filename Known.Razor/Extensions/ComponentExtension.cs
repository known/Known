namespace Known.Razor.Extensions;

public static class ComponentExtension
{
    public static void Badge(this RenderTreeBuilder builder, StyleType style, string text)
    {
        builder.Component<Badge>().Set(c => c.Style, style).Set(c => c.Text, text).Build();
    }

    public static void Tag(this RenderTreeBuilder builder, StyleType style, string text)
    {
        builder.Component<Tag>().Set(c => c.Style, style).Set(c => c.Text, text).Build();
    }

    public static void Tag(this RenderTreeBuilder builder, StyleType style, Action<RenderTreeBuilder> content)
    {
        builder.Component<Tag>().Set(c => c.Style, style).Set(c => c.Content, content).Build();
    }
}