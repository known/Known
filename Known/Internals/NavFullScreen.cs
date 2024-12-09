namespace Known.Internals;

[NavItem]
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