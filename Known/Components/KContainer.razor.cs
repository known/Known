namespace Known.Components;

/// <summary>
/// 全局容器组件类。
/// </summary>
public partial class KContainer
{
    [Inject] private IJSRuntime Runtime { get; set; }

    private bool reconnectInit;
    private string ModeName => Runtime.IsServerMode() ? "Server" : "Wasm";

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (reconnectInit || !Runtime.IsServerMode())
            return;

        try
        {
            await Runtime.InvokeVoidAsync("KUtils.setupReconnectAutoLogin", "/login", 1000);
            reconnectInit = true;
        }
        catch (JSException)
        {
            await Task.Delay(300);
            await InvokeAsync(StateHasChanged);
        }
    }
}