namespace Known.Components;

/// <summary>
/// 锚点组件类。
/// </summary>
public class KAnchor : BaseComponent
{
    /// <summary>
    /// 取得或设置连接地址。
    /// </summary>
    [Parameter] public string Href { get; set; }

    /// <summary>
    /// 取得或设置下载文件名。
    /// </summary>
    [Parameter] public string Download { get; set; }

    /// <summary>
    /// 取得或设置打开方式。
    /// </summary>
    [Parameter] public string Target { get; set; }

    /// <summary>
    /// 取得或设置连接子内容模板。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        var className = CssBuilder.Default("ant-btn-link").AddClass(Class).BuildClass();
        builder.Element("a").Class(className).Style(Style).Href(Href)
               .Set("target", Target)
               .Set("download", Download)
               .Child(() =>
               {
                   if (ChildContent != null)
                       builder.Fragment(ChildContent);
                   else
                       builder.Text(Name);
               });
    }
}