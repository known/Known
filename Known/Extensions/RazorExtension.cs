namespace Known.Extensions;

public static class RazorExtension
{
    #region Button
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
    #endregion

    #region Banner
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
    #endregion

    #region Badge
    public static void Badge(this RenderTreeBuilder builder, StyleType style, string text)
    {
        builder.Component<KBadge>()
               .Set(c => c.Style, style)
               .Set(c => c.Text, text)
               .Build();
    }
    #endregion

    #region Tag
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
    #endregion

    #region Progress
    public static void Progress(this RenderTreeBuilder builder, StyleType style, decimal value, int? width = null)
    {
        builder.Component<KProgress>()
               .Set(c => c.Style, style)
               .Set(c => c.Value, value)
               .Set(c => c.Width, width)
               .Build();
    }
    #endregion

    #region Dropdown
    public static void Dropdown(this RenderTreeBuilder builder, List<KMenuItem> items, string text = null, string style = null)
    {
        builder.Component<KDropdown>()
               .Set(c => c.Style, style)
               .Set(c => c.Text, text)
               .Set(c => c.Items, items)
               .Build();
    }
    #endregion

    #region Barcode/QRCode
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
    #endregion

    #region View
    public static void ViewLR(this RenderTreeBuilder builder, Action<AttributeBuilder> left, Action<AttributeBuilder> right)
    {
        builder.Div("lr-view", attr =>
        {
            builder.Div("left-view", left);
            builder.Div("right-view", right);
        });
    }

    public static void StatusTag(this RenderTreeBuilder builder, string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return;

        var style = StyleType.Default;
        if (status.Contains("待") || status.Contains("中"))
            style = StyleType.Info;
        else if (status.Contains("完成"))
            style = StyleType.Primary;
        else if (status.Contains("重新"))
            style = StyleType.Warning;
        else if (status.Contains("退回") || status.Contains("不通过") || status.Contains("失败"))
            style = StyleType.Danger;
        else if (status.Contains("已") || status.Contains("通过") || status.Contains("成功") || status == "正常")
            style = StyleType.Success;
        builder.Tag(style, status);
    }
    #endregion

    #region Icon
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
    #endregion

    #region Form
    public static void Form(this RenderTreeBuilder builder, Action<RenderTreeBuilder> body, Action<RenderTreeBuilder> action)
    {
        builder.Div("form", attr =>
        {
            builder.Div("form-body", attr => body.Invoke(builder));
            builder.Div("form-button", attr => action.Invoke(builder));
        });
    }

    public static void FormList<T>(this RenderTreeBuilder builder, string title, string style = null, Action<ComponentBuilder<T>> child = null) where T : notnull, IComponent
    {
        builder.FormList(title, style, () => builder.Component(child));
    }

    public static void FormList(this RenderTreeBuilder builder, string title, string style = null, Action child = null)
    {
        builder.Div("form-caption", title);
        var css = CssBuilder.Default("form-list").AddClass(style).Build();
        builder.Div(css, attr => child.Invoke());
    }
    #endregion

    #region Field
    public static void Hidden(this RenderTreeBuilder builder, string id) => builder.Field<KHidden>(id).Build();
    public static void Hidden(this RenderTreeBuilder builder, string id, string value) => builder.Field<KHidden>(id).Value(value).Build();
    public static FieldAttrBuilder<T> Field<T>(this RenderTreeBuilder builder, string id, bool required = false) where T : Field => builder.Field<T>("", id, required);

    public static FieldAttrBuilder<T> Field<T>(this RenderTreeBuilder builder, string label, string id, bool required = false) where T : Field
    {
        var fb = new FieldAttrBuilder<T>(builder);
        fb.Set(f => f.Label, label)
          .Set(f => f.Id, id)
          .Set(f => f.Required, required);
        if (typeof(T) == typeof(KSelect))
            fb.Add(nameof(KSelect.EmptyText), "请选择");
        return fb;
    }
    #endregion
}