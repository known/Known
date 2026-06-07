namespace Known.Internals;

/// <summary>
/// 全屏组件类。
/// </summary>
[NavPlugin(Language.NavFullScreen, "fullscreen", Category = Language.Component, Sort = 3)]
public class NavFullScreen : BaseNav
{
    private bool isFullScreen = false;

    /// <inheritdoc />
    protected override string Title => isFullScreen ? Language.ExitScreen : Language.FullScreen;
    
    /// <inheritdoc />
    protected override string Icon => isFullScreen ? "fullscreen-exit" : "fullscreen";

    /// <inheritdoc />
    protected override EventCallback<MouseEventArgs> OnClick => this.Callback<MouseEventArgs>(e => OnFullScreenAsync());

    private async Task OnFullScreenAsync()
    {
        isFullScreen = !isFullScreen;
        if (isFullScreen)
            await JS.OpenFullScreenAsync();
        else
            await JS.CloseFullScreenAsync();
    }
}