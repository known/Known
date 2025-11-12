namespace Known.Blazor;

public partial class JSService
{
    /// <summary>
    /// 异步全屏显示系统。
    /// </summary>
    /// <returns></returns>
    public Task OpenFullScreenAsync()
    {
        return InvokeAsync("KBlazor.openFullScreen");
    }

    /// <summary>
    /// 异步关闭全屏显示。
    /// </summary>
    /// <returns></returns>
    public Task CloseFullScreenAsync()
    {
        return InvokeAsync("KBlazor.closeFullScreen");
    }
}