namespace Known.Components;

/// <summary>
/// 全屏组件类。
/// </summary>
public partial class KFullScreen
{
    private bool isFullScreen = false;
    private string Title => isFullScreen ? Language.ExitScreen : Language.FullScreen;
    private string Icon => isFullScreen ? "fullscreen-exit" : "fullscreen";
    private string ClassName => CssBuilder.Default("kui-screen")
                                          .AddClass("is-full", isFullScreen)
                                          .BuildClass();

    /// <summary>
    /// 取得或设置全屏后的委托。
    /// </summary>
    [Parameter] public Func<Task> OnFullScreen { get; set; }

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
        if (OnFullScreen != null)
            await OnFullScreen.Invoke();
    }
}