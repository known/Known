using Known.Razor;

namespace Known.Extensions;

public static class RazorExtension
{
    public static void Banner(this RenderTreeBuilder builder, StyleType style, string text)
    {
        builder.Component<KBanner>()
               .Set(c => c.Content, b => b.Text(text))
               .Set(c => c.Style, style)
               .Build();
    }

    public static void Banner(this RenderTreeBuilder builder, StyleType style, Action<RenderTreeBuilder> content)
    {
        builder.Component<KBanner>()
               .Set(c => c.Content, content)
               .Set(c => c.Style, style)
               .Build();
    }

    public static void Badge(this RenderTreeBuilder builder, StyleType style, string text)
    {
        builder.Component<KBadge>()
               .Set(c => c.Style, style)
               .Set(c => c.Text, text)
               .Build();
    }

    public static void Tag(this RenderTreeBuilder builder, StyleType style, string text, Action onClick = null)
    {
        builder.Component<KTag>()
               .Set(c => c.Style, style)
               .Set(c => c.Text, text)
               .Set(c => c.OnClick, onClick)
               .Build();
    }

    public static void Tag(this RenderTreeBuilder builder, StyleType style, Action<RenderTreeBuilder> content, Action onClick = null)
    {
        builder.Component<KTag>()
               .Set(c => c.Style, style)
               .Set(c => c.Content, content)
               .Set(c => c.OnClick, onClick)
               .Build();
    }

    public static void Progress(this RenderTreeBuilder builder, StyleType style, decimal value, int? width = null)
    {
        builder.Component<KProgress>()
               .Set(c => c.Style, style)
               .Set(c => c.Value, value)
               .Set(c => c.Width, width)
               .Build();
    }

    public static void Dropdown(this RenderTreeBuilder builder, List<KMenuItem> items, string text = null, string style = null)
    {
        builder.Component<KDropdown>()
               .Set(c => c.Style, style)
               .Set(c => c.Text, text)
               .Set(c => c.Items, items)
               .Build();
    }

    public static void Barcode(this RenderTreeBuilder builder, string id, string value, object option = null)
    {
        builder.Component<KBarcode>().Id(id)
               .Set(c => c.Value, value)
               .Set(c => c.Option, option)
               .Build();
    }

    public static void QRCode(this RenderTreeBuilder builder, string id, object option = null)
    {
        builder.Component<KQRCode>().Id(id)
               .Set(c => c.Option, option)
               .Build();
    }
}