namespace Known.Components;

/// <summary>
/// 全局容器组件类。
/// </summary>
public partial class KContainer
{
    [Inject] private IJSRuntime Runtime { get; set; }

    private string ModeName => Runtime.IsServerMode() ? "Server" : "Wasm";
}