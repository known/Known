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
    private string scanResult = string.Empty;
    private string currentScan = string.Empty;
    private ElementReference scannerInput;
    private System.Timers.Timer scanTimer;

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

        await StartPDAAsync();
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
            scanTimer.Stop();
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

    /// <inheritdoc />
    protected override async Task OnDisposeAsync()
    {
        await base.OnDisposeAsync();
        scanTimer?.Dispose();
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
        //builder.AddAttribute(1, "readonly", true);
        builder.AddAttribute(1, "style", "opacity:0;position:absolute;left:-1000px;");
        builder.AddAttribute(2, "value", BindConverter.FormatValue(scanResult));
        builder.AddAttribute(3, "onkeypress", EventCallback.Factory.Create(this, (Action<KeyboardEventArgs>)HandleKeyPress));
        builder.AddAttribute(4, "onchange", EventCallback.Factory.CreateBinder(this, delegate (string value) { scanResult = value; }, scanResult));
        builder.SetUpdatesAttributeName("value");
        builder.AddElementReferenceCapture(5, delegate (ElementReference value) { scannerInput = value; });
        builder.CloseElement();

        if (isScanning)
            builder.Span(Language["请按PDA扫描键，若无结果，请点击此处再按。"], this.Callback<MouseEventArgs>(e => StartPDAAsync()));
        else
            builder.Span(Language["点击下方按钮开始扫描二维码"]);
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
            builder.Span(Language["点击下方按钮开始扫描二维码"]);
        }
    }

    private async Task StartPDAAsync()
    {
        await JSRuntime.InvokeJsAsync("KUtils.scanPDA", scannerInput);
        scanTimer = new System.Timers.Timer(100);
        scanTimer.Elapsed += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(currentScan) && currentScan == PDAEnd)
            {
                InvokeAsync(async () =>
                {
                    await OnScanned(scanResult, "");
                });
                currentScan = string.Empty;
            }
        };
        scanTimer.Start();
    }

    private void HandleKeyPress(KeyboardEventArgs e)
    {
        currentScan = e.Key;
    }
}