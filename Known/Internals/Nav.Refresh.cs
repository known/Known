namespace Known.Internals;

/// <summary>
/// 页面刷新组件。
/// </summary>
[NavPlugin(Language.NavRefresh, "reload", Category = Language.Component, Sort = 2)]
public class NavRefresh : BaseNav
{
    /// <inheritdoc />
    protected override string Title => Language.RefreshPage;

    /// <inheritdoc />
    protected override string Icon => "reload";

    /// <inheritdoc />
    protected override EventCallback<MouseEventArgs> OnClick => this.Callback<MouseEventArgs>(e => OnRefresh());

    private void OnRefresh()
    {
        App?.ReloadPage();
    }
}