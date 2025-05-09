namespace Known.Components;

/// <summary>
/// 条形码组件类。
/// </summary>
public partial class KBarcode
{
    //条形码组件类，配置参考barcode.js。
    private string lastCode;

    /// <summary>
    /// 取得或设置条形码组件条码值。
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <summary>
    /// 取得或设置条形码组件条码对象，参考Barcode.js配置。
    /// </summary>
    [Parameter] public object Option { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Id = $"bc-{Id}";
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if ((firstRender || Value != lastCode) && Visible)
        {
            lastCode = Value;
            await JS.ShowBarcodeAsync(Id, Value, Option);
        }
    }
}