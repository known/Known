namespace Known.Internals;

/// <summary>
/// 页面刷新组件。
/// </summary>
public class NavRefresh : BaseNav
{
    /// <inheritdoc />
    protected override string Title => Language["Nav.RefreshPage"];

    /// <inheritdoc />
    protected override string Icon => "reload";

    /// <inheritdoc />
    protected override EventCallback<MouseEventArgs> OnClick => this.Callback<MouseEventArgs>(e => OnRefresh());

    private void OnRefresh()
    {
        App?.ReloadPage();
    }
}