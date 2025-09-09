using AntDesign;

namespace Known.Internals;

class Scanner : BaseComponent
{
    private readonly string cameraId = "kuiCamera";
    private string errorMessage = string.Empty;

    [Parameter] public Func<string, string, Task> OnScan { get; set; }
    [Parameter] public Func<Task> OnStop { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!Visible)
            return;

        if (string.IsNullOrWhiteSpace(errorMessage))
        {
            builder.Element("video").Id(cameraId).Close();
            return;
        }

        builder.Component<Alert>()
               .Set(c => c.Type, AlertType.Error)
               .Set(c => c.Style, "height:100%;padding-top:40px;")
               .Set(c => c.ShowIcon, true)
               .Set(c => c.Message, Language[Language.Error])
               .Set(c => c.Description, errorMessage)
               .Build();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender && Visible)
        {
            var invoker = DotNetObjectReference.Create(this);
            await JSRuntime.InvokeVoidAsync("KUtils.scanStart", invoker, cameraId);
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

    [JSInvokable]
    public void OnError(string error)
    {
        errorMessage = error;
        StateChanged();
    }
}