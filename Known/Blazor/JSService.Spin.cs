namespace Known.Blazor;

public partial class JSService
{
    /// <summary>
    /// 异步显示加载中。
    /// </summary>
    /// <param name="tip">提示信息。</param>
    /// <returns></returns>
    public Task ShowSpinAsync(string tip = null)
    {
        return InvokeAsync("KBlazor.showSpin", tip);
    }

    /// <summary>
    /// 异步隐藏加载中。
    /// </summary>
    /// <returns></returns>
    public Task HideSpinAsync()
    {
        return InvokeAsync("KBlazor.hideSpin");
    }
}