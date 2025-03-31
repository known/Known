namespace Known.Components;

/// <summary>
/// 分组框组件类。
/// </summary>
public partial class KGroupBox
{
    /// <summary>
    /// 取得或设置分组框标题。
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// 取得或设置分组框子内容。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }
}