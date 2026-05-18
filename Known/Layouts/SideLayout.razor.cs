namespace Known.Layouts;

/// <summary>
/// 简洁边栏模板组件类。
/// </summary>
public partial class SideLayout
{
    /// <summary>
    /// 取得或设置子组件内容。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }
}