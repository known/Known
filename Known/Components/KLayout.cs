namespace Known.Components;

/// <summary>
/// 布局组件类。
/// </summary>
public class KLayout : BaseComponent
{
    /// <summary>
    /// 取得或设置布局样式CSS类名。
    /// </summary>
    [Parameter] public string Class { get; set; }

    /// <summary>
    /// 取得或设置模板基类对象。
    /// </summary>
    [Parameter] public BaseLayout Layout { get; set; }

    /// <summary>
    /// 取得或设置布局子内容。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// 呈现布局组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        var className = CssBuilder.Default("kui-wrapper").AddClass(Class).BuildClass();
        builder.Div(className, () =>
        {
            builder.Cascading(Layout, ChildContent);
        });
    }
}