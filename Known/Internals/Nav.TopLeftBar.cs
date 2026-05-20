namespace Known.Internals;

/// <summary>
/// 全局顶部导航工具条组件类。
/// </summary>
public class TopLeftBar : BaseComponent
{
    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (UIConfig.TopLeft != null)
        {
            builder.Fragment(UIConfig.TopLeft);
            return;
        }

        builder.Component<NavRefresh>().Build();
        builder.Component<TopBreadcrumb>().Build();
    }
}