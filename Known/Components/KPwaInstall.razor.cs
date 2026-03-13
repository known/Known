namespace Known.Components;

/// <summary>
/// 安装PWA到主屏幕按钮组件类。
/// </summary>
public partial class KPwaInstall
{
    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        Id = "btnPwaInstall";
        await base.OnInitAsync();
    }

    /// <inheritdoc />
    protected override async Task OnRenderAsync(bool firstRender)
    {
        await base.OnRenderAsync(firstRender);
        if (firstRender && Visible && Config.App.Type == AppType.Web)
        {
            await JSRuntime.InvokeJsAsync("initializePwaInstallButton", Id);
        }
    }
}