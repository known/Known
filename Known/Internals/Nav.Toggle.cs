namespace Known.Internals;

/// <summary>
/// 切换边栏菜单组件。
/// </summary>
public class NavToggle : BaseNav
{
    private bool collapsed;

    /// <summary>
    /// 取得或设置切换按钮点击事件委托。
    /// </summary>
    [Parameter] public EventCallback<bool> OnToggle { get; set; }

    /// <inheritdoc />
    protected override string Title => collapsed ? Language["Nav.Expand"] : Language["Nav.Collapse"];

    /// <inheritdoc />
    protected override string Icon => collapsed ? "menu-unfold" : "menu-fold";

    /// <inheritdoc />
    protected override EventCallback<MouseEventArgs> OnClick => this.Callback<MouseEventArgs>(e => OnItemToggle());

    /// <summary>
    /// 异步切换菜单栏。
    /// </summary>
    /// <param name="collapsed">是否折叠。</param>
    public void Toggle(bool collapsed)
    {
        this.collapsed = collapsed;
        if (OnToggle.HasDelegate)
            OnToggle.InvokeAsync(collapsed);
    }

    private async Task OnItemToggle()
    {
        collapsed = !collapsed;
        if (OnToggle.HasDelegate)
            await OnToggle.InvokeAsync(collapsed);
    }
}