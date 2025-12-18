namespace Known.Components;

/// <summary>
/// 扫码组件类。
/// </summary>
public class KScanner : BaseComponent
{
    private const string TipPDA = "请按PDA扫描键进行扫描！";
    private const string TipScan = "点击下方按钮开始扫描二维码";
    private DotNetObjectReference<KScanner> invoker;
    private readonly string cameraId = "kuiCamera";
    private bool isScanning;
    private string errorMessage = string.Empty;
    private ElementReference scannerInput;

    /// <summary>
    /// 取得是否正在扫码。
    /// </summary>
    public bool IsScanning => isScanning;

    /// <summary>
    /// 取得或设置是否自动开始。
    /// </summary>
    [Parameter] public bool AutoStart { get; set; }

    /// <summary>
    /// 取得或设置是否使用PDA扫码器。
    /// </summary>
    [Parameter] public bool IsPDA { get; set; }

    /// <summary>
    /// 取得或设置PDA扫码结束字符，默认：Enter。
    /// </summary>
    [Parameter] public string PDAEnd { get; set; } = "Enter";

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
    public async Task StartAsync()
    {
        if (isScanning)
            return;

        isScanning = true;
        if (!IsPDA)
        {
            await JSRuntime.InvokeJsAsync("KUtils.scanStart", invoker, cameraId);
            return;
        }

        await JSRuntime.InvokeJsAsync("KUtils.scanPDA", invoker, scannerInput);
    }

    /// <summary>
    /// 异步停止扫码。
    /// </summary>
    /// <returns></returns>
    public async Task StopAsync()
    {
        isScanning = false;
        if (!IsPDA)
            await JSRuntime.InvokeJsAsync("KUtils.scanStop");
        else
            await JSRuntime.InvokeJsAsync("KUtils.stopPDA");
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

        var className = CssBuilder.Default("kui-scanner").AddClass("pda", IsPDA).BuildClass();
        builder.Div(className, () =>
        {
            if (IsPDA)
                BuildPDA(builder);
            else
                BuildCamera(builder);
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

    private void BuildPDA(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "input");
        builder.AddAttribute(1, "readonly", true);
        builder.AddAttribute(2, "style", "opacity:0;position:absolute;left:-1000px;");
        builder.SetUpdatesAttributeName("value");
        builder.AddElementReferenceCapture(4, delegate (ElementReference value) { scannerInput = value; });
        builder.CloseElement();

        if (isScanning)
            builder.Span(Language[TipPDA]);
        else
            builder.Span(Language[TipScan]);
    }

    private void BuildCamera(RenderTreeBuilder builder)
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
            builder.Span(Language[TipScan]);
        }
    }
}