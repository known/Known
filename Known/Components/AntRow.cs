namespace Known.Components;

/// <summary>
/// 自定义Ant行组件类。
/// </summary>
public class AntRow : ComponentBase
{
    /// <summary>
    /// 取得或设置行内的列之间的排水沟宽度。
    /// </summary>
    [Parameter] public int Gutter { get; set; }

    /// <summary>
    /// 取得或设置行的CSS类名。
    /// </summary>
    [Parameter] public string Class { get; set; }

    /// <summary>
    /// 取得或设置行的样式字符串。
    /// </summary>
    [Parameter] public string Style { get; set; }

    /// <summary>
    /// 取得或设置行的子内容。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// 呈现列组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var className = CssBuilder.Default("ant-row")
                          .AddClass(Class)
                          .BuildClass();
        var style = CssBuilder.Default().AddStyle(Style);
        if (Gutter > 0)
        {
            style.Add("margin-left", $"-{Gutter / 2}px");
            style.Add("margin-right", $"-{Gutter / 2}px");
        }
        builder.Div().Class(className).Style(style.BuildStyle()).Children(() =>
        {
            builder.Cascading(this, b => b.Fragment(ChildContent));
        }).Close();
    }
}