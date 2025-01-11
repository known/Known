namespace Known.Blazor;

/// <summary>
/// 顶部导航组件基类。
/// </summary>
public class BaseNav : BaseComponent
{
    /// <summary>
    /// 取得提示文本。
    /// </summary>
    protected virtual string Title { get; }

    /// <summary>
    /// 取得图标。
    /// </summary>
    protected virtual string Icon { get; }

    /// <summary>
    /// 取得单击事件。
    /// </summary>
    protected virtual EventCallback<MouseEventArgs> OnClick { get; }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Component<KIcon>()
               .Set(c => c.Title, Title)
               .Set(c => c.Icon, Icon)
               .Set(c => c.OnClick, OnClick)
               .Build();
    }
}