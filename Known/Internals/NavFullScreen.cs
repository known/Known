namespace Known.Internals;

[Plugin(PluginType.Navbar, "全屏", Category = "组件", Icon = "fullscreen")]
class NavFullScreen : BaseNav
{
    private bool isFullScreen = false;

    protected override string Title => isFullScreen ? Language["Nav.ExitScreen"] : Language["Nav.FullScreen"];
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