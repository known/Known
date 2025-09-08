namespace Known.Internals;

class Scanner : BaseComponent
{
    private readonly string cameraId = "kuiCamera";

    [Parameter] public Func<string, string, Task> OnScan { get; set; }
    [Parameter] public Func<Task> OnStop { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!Visible)
            return;

        builder.Element("video").Id(cameraId).Close();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender && Visible)
        {
            var invoker = DotNetObjectReference.Create(this);
            await JSRuntime.InvokeVoidAsync("KUtils.scanStart", cameraId, invoker);
        }
    }

    internal async Task StopAsync()
    {
        await JSRuntime.InvokeVoidAsync("KUtils.scanStop");
    }

    [JSInvokable]
    public async Task OnScanned(string text, string error)
    {
        Visible = false;
        if (OnScan != null)
            await OnScan.Invoke(text, error);
    }

    [JSInvokable]
    public async Task OnScanStop()
    {
        Visible = false;
        if (OnStop != null)
            await OnStop.Invoke();
    }
}