namespace Known.AntBlazor.Components;

/// <summary>
/// 自定义Ant列组件类。
/// </summary>
public class AntCol : ComponentBase
{
    [CascadingParameter] private AntRow Row { get; set; }

    /// <summary>
    /// 取得或设置列的跨度。
    /// </summary>
    [Parameter] public int Span { get; set; }

    /// <summary>
    /// 取得或设置列的补偿宽度。
    /// </summary>
    [Parameter] public int Offset { get; set; }

    /// <summary>
    /// 取得或设置列的CSS类名。
    /// </summary>
    [Parameter] public string Class { get; set; }

    /// <summary>
    /// 取得或设置列的样式字符串。
    /// </summary>
    [Parameter] public string Style { get; set; }

    /// <summary>
    /// 取得或设置列的子内容。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// 呈现列组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var className = CssBuilder.Default("ant-col")
                          .AddClass($"ant-col-{Span}", Span > 0)
                          .AddClass($"ant-col-offset-{Offset}", Offset > 0)
                          .AddClass(Class)
                          .BuildClass();
        var style = CssBuilder.Default().AddStyle(Style);
        if (Row != null && Row.Gutter > 0)
        {
            style.Add("padding-left", $"{Row.Gutter / 2}px");
            style.Add("padding-right", $"{Row.Gutter / 2}px");
        }
        builder.Div().Class(className).Style(style.BuildStyle())
               .Children(() => builder.Fragment(ChildContent))
               .Close();
    }
}