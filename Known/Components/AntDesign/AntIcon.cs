using AntDesign;

namespace Known.Components;

/// <summary>
/// 扩展Ant图标组件类。
/// </summary>
public class AntIcon : Icon
{
    /// <summary>
    /// 取得或设置图标提示标题。
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// 取得或设置图标颜色是否跟随主题色。
    /// </summary>
    [Parameter] public bool IsTheme { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Class = CssBuilder.Default(Class).AddClass("ant-btn-link", IsTheme).BuildClass();
        base.OnInitialized();
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            base.BuildRenderTree(builder);
            return;
        }

        builder.Tooltip(Title, base.BuildRenderTree);
    }
}