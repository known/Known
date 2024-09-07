namespace Known.Components;

/// <summary>
/// 布局组件类。
/// </summary>
public class KLayout : BaseComponent
{
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
        builder.Component<KError>()
               .Set(c => c.OnError, Layout.OnError)
               .Set(c => c.ChildContent, b => b.Cascading(Layout, ChildContent))
               .Build();
    }
}