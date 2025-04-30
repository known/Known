namespace Known.Internals;

[NavPlugin("全屏", "fullscreen", Category = "组件", Sort = 2)]
class NavFullScreen : BaseNav
{
    private bool isFullScreen = false;

    protected override string Title => isFullScreen ? Language.ExitScreen : Language.FullScreen;
    protected override string Icon => isFullScreen ? "fullscreen-exit" : "fullscreen";

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