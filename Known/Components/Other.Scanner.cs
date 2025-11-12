namespace Known.Components;

/// <summary>
/// 扫码组件类。
/// </summary>
public class KScanner : BaseComponent
{
    private DotNetObjectReference<KScanner> invoker;
    private readonly string cameraId = "kuiCamera";
    private bool isScanning;
    private string errorMessage = string.Empty;

    /// <summary>
    /// 取得是否正在扫码。
    /// </summary>
    public bool IsScanning => isScanning;

    /// <summary>
    /// 取得或设置是否自动开始。
    /// </summary>
    [Parameter] public bool AutoStart { get; set; }

    /// <summary>
    /// 取得或设置扫码成功委托。
    /// </summary>
    [Parameter] public Func<string, string, Task> OnScan { get; set; }

    /// <summary>
    /// 取得或设置停止扫码委托。
    /// </summary>
    [Parameter] public Func<Task> OnStop { get; set; }

    /// <summary>
    /// 异步开始扫码。
    /// </summary>
    /// <returns></returns>
    public Task StartAsync()
    {
        if (isScanning)
            return Task.CompletedTask;

        isScanning = true;
        return JSRuntime.InvokeJsAsync("KUtils.scanStart", invoker, cameraId);
    }

    /// <summary>
    /// 异步停止扫码。
    /// </summary>
    /// <returns></returns>
    public Task StopAsync()
    {
        isScanning = false;
        return JSRuntime.InvokeJsAsync("KUtils.scanStop");
    }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        invoker = DotNetObjectReference.Create(this);
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!Visible)
            return;

        builder.Div("kui-scanner", () =>
        {
            if (isScanning)
            {
                builder.Element("video").Id(cameraId).Close();
                builder.Div("scan-line", "");
            }
            else if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                builder.Span("error", Language[errorMessage]);
            }
            else
            {
                builder.Span("点击下方按钮开始扫描二维码");
            }
        });
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender && Visible && AutoStart)
        {
            await StartAsync();
        }
    }

    /// <summary>
    /// 扫码成功回调。
    /// </summary>
    /// <param name="text">扫码内容。</param>
    /// <param name="error">错误信息。</param>
    /// <returns></returns>
    [JSInvokable]
    public async Task OnScanned(string text, string error)
    {
        await StopAsync();
        if (OnScan != null)
            await OnScan.Invoke(text, error);
    }

    /// <summary>
    /// 停止扫码回调。
    /// </summary>
    /// <returns></returns>
    [JSInvokable]
    public async Task OnScanStop()
    {
        if (OnStop != null)
            await OnStop.Invoke();
    }

    /// <summary>
    /// 扫码错误回调。
    /// </summary>
    /// <param name="error">错误信息。</param>
    [JSInvokable]
    public void OnError(string error)
    {
        isScanning = false;
        errorMessage = error;
        StateChanged();
    }
}