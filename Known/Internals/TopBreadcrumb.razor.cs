namespace Known.Internals;

/// <summary>
/// 顶部面包屑组件类。
/// </summary>
public partial class TopBreadcrumb
{
    /// <summary>
    /// 取得或设置当前菜单信息。
    /// </summary>
    [Parameter] public MenuInfo Current { get; set; }

    /// <summary>
    /// 取得或设置首页点击委托。
    /// </summary>
    [Parameter] public EventCallback OnHome { get; set; }

    private void OnHomeClick()
    {
        if (OnHome.HasDelegate)
            OnHome.InvokeAsync();
        else
            Navigation.GoHomePage();
    }
}