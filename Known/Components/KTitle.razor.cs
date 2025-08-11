namespace Known.Components;

/// <summary>
/// 标题组件类。
/// </summary>
public partial class KTitle
{
    private string ClassName => CssBuilder.Default("kui-title").AddClass(Class).BuildClass();

    /// <summary>
    /// 取得或设置组件标题文本。
    /// </summary>
    [Parameter] public string Text { get; set; }

    /// <summary>
    /// 取得或设置组件子标题文本。
    /// </summary>
    [Parameter] public string SubText { get; set; }

    /// <summary>
    /// 取得或设置标题组件子内容。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }
}