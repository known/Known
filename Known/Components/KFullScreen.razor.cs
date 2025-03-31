namespace Known.Components;

/// <summary>
/// 全屏组件类。
/// </summary>
public partial class KFullScreen
{
    private bool isFullScreen = false;
    private string Title => isFullScreen ? Language["Nav.ExitScreen"] : Language["Nav.FullScreen"];
    private string Icon => isFullScreen ? "fullscreen-exit" : "fullscreen";
    private string Class => CssBuilder.Default("kui-screen")
                                      .AddClass("is-full", isFullScreen)
                                      .BuildClass();

    /// <summary>
    /// 取得或设置子內容组件。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    private async Task OnFullScreenAsync(MouseEventArgs args)
    {
        isFullScreen = !isFullScreen;
        if (isFullScreen)
            await JS.OpenFullScreenAsync();
        else
            await JS.CloseFullScreenAsync();
    }
}