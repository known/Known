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

    public static void Progress(this RenderTreeBuilder builder, StyleType style, int width, decimal value)
    {
        builder.Component<Progress>().Set(c => c.Style, style).Set(c => c.Width, width).Set(c => c.Value, value).Build();
    }
}