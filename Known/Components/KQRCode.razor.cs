namespace Known.Components;

/// <summary>
/// 二维码组件类，配置参考qrcode.js。
/// </summary>
public partial class KQRCode
{
    private string ClassName => CssBuilder.Default("kui-qrcode").AddClass(Class).BuildClass();

    /// <summary>
    /// 取得或设置二维码配置对象，配置参考qrcode.js。。
    /// </summary>
    [Parameter] public object Option { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Id = $"qr-{Id}";
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender && Visible)
            await JS.ShowQRCodeAsync(Id, Option);
    }
}