namespace Known.Razor.Extensions;

public static class RazorExtension
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

    public static void Dropdown(this RenderTreeBuilder builder, List<MenuItem> items, string text = null, string style = null)
    {
        builder.Component<Dropdown>().Set(c => c.Style, style).Set(c => c.Text, text).Set(c => c.Items, items).Build();
    }
}