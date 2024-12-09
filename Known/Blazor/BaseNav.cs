using AntDesign;

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

    /// <summary>
    /// 呈现组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Component<Tooltip>()
               .Set(c => c.Title, Title)
               .Set(c => c.ChildContent, BuildIcon)
               .Build();
    }

    private void BuildIcon(RenderTreeBuilder builder)
    {
        builder.Component<Icon>()
               .Set(c => c.Type, Icon)
               .Set(c => c.OnClick, OnClick)
               .Build();
    }
}