namespace Known.Extensions;

/// <summary>
/// HTML呈现扩展类。
/// </summary>
public static class HtmlExtension
{
    /// <summary>
    /// 呈现一个label元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="text">label的文本，支持html字符串。</param>
    public static void Label(this RenderTreeBuilder builder, string text)
    {
        builder.Label(() => builder.Markup(text));
    }

    /// <summary>
    /// 呈现一个label元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="className">div的CSS类名。</param>
    /// <param name="text">label的文本，支持html字符串。</param>
    public static void Label(this RenderTreeBuilder builder, string className, string text)
    {
        builder.Label().Class(className).Child(() => builder.Markup(text));
    }

    /// <summary>
    /// 呈现一个label元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="child">label的子元素委托。</param>
    public static void Label(this RenderTreeBuilder builder, Action child)
    {
        builder.Label().Child(child);
    }

    /// <summary>
    /// 呈现一个div元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="child">div的子元素委托。</param>
    public static void Div(this RenderTreeBuilder builder, Action child)
    {
        builder.Div("", child);
    }

    /// <summary>
    /// 呈现一个div元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="className">div的CSS类名。</param>
    /// <param name="text">div的文本内容，支持html字符串。</param>
    public static void Div(this RenderTreeBuilder builder, string className, string text)
    {
        builder.Div(className, () => builder.Markup(text));
    }

    /// <summary>
    /// 呈现一个div元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="className">div的CSS类名。</param>
    /// <param name="child">div的子元素委托。</param>
    public static void Div(this RenderTreeBuilder builder, string className, Action child)
    {
        builder.Div().Class(className).Child(child);
    }

    /// <summary>
    /// 呈现一个ul元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="child">ul的子元素委托。</param>
    public static void Ul(this RenderTreeBuilder builder, Action child)
    {
        builder.Ul("", child);
    }

    /// <summary>
    /// 呈现一个ul元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="className">ul的CSS类名。</param>
    /// <param name="child">ul的子元素委托。</param>
    public static void Ul(this RenderTreeBuilder builder, string className, Action child)
    {
        builder.Ul().Class(className).Child(child);
    }

    /// <summary>
    /// 呈现一个li元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="child">li的子元素委托。</param>
    public static void Li(this RenderTreeBuilder builder, Action child)
    {
        builder.Li("", child);
    }

    /// <summary>
    /// 呈现一个li元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="className">li的CSS类名。</param>
    /// <param name="text">li的文本，支持html字符串。</param>
    public static void Li(this RenderTreeBuilder builder, string className, string text)
    {
        builder.Li(className, () => builder.Markup(text));
    }

    /// <summary>
    /// 呈现一个li元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="className">li的CSS类名。</param>
    /// <param name="child">li的子元素委托。</param>
    public static void Li(this RenderTreeBuilder builder, string className, Action child)
    {
        builder.Li().Class(className).Child(child);
    }

    /// <summary>
    /// 呈现一个span元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="text">span的文本。</param>
    /// <param name="onClick">span的单击事件。</param>
    public static void Span(this RenderTreeBuilder builder, string text, EventCallback<MouseEventArgs>? onClick = null)
    {
        builder.Span("", text, onClick);
    }

    /// <summary>
    /// 呈现一个span元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="className">span的CSS类名。</param>
    /// <param name="text">span的文本。</param>
    /// <param name="onClick">span的单击事件。</param>
    public static void Span(this RenderTreeBuilder builder, string className, string text, EventCallback<MouseEventArgs>? onClick = null)
    {
        builder.Span().Class(className).OnClick(onClick).Markup(text);
    }

    /// <summary>
    /// 呈现一个iframe元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="url">iframe的URL。</param>
    public static void IFrame(this RenderTreeBuilder builder, string url)
    {
        builder.IFrame().Class("kui-frame").Src(url).Close();
    }

    /// <summary>
    /// 呈现一个链接样式的span元素。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="text">span文本。</param>
    /// <param name="onClick">span单击事件。</param>
    public static void Link(this RenderTreeBuilder builder, string text, EventCallback onClick)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        builder.Span().Class("ant-btn-link").OnClick(onClick).Markup(text);
    }

    /// <summary>
    /// 呈现一个下载附件的链接。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="text">链接文本。</param>
    /// <param name="url">链接URL。</param>
    public static void OpenFile(this RenderTreeBuilder builder, string text, FileUrlInfo url)
    {
        builder.Component<KAnchor>()
               .Set(c => c.Name, text)
               .Set(c => c.Href, url.OriginalUrl)
               .Set(c => c.Download, url.FileName)
               .Set(c => c.Target, "_blank")
               .Build();
    }
}