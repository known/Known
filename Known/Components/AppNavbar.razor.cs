namespace Known.Components;

/// <summary>
/// 移动端导航栏组件类。
/// </summary>
public partial class AppNavbar
{
    /// <summary>
    /// 取得或设置是否显示返回按钮。
    /// </summary>
    [Parameter] public bool ShowBack { get; set; }

    /// <summary>
    /// 取得或设置返回按钮点击事件回调。
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> OnBack { get; set; }
}