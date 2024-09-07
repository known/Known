namespace Known.Components;

/// <summary>
/// 条形码组件类，配置参考barcode.js。
/// </summary>
public class KBarcode : BaseComponent
{
    private readonly string id;
    private string lastCode;

    /// <summary>
    /// 构造函数，创建一个条形码组件类的实例。
    /// </summary>
    public KBarcode()
    {
        id = Utils.GetGuid();
        id = $"bc-{id}";
    }

    /// <summary>
    /// 取得或设置条形码组件样式。
    /// </summary>
    [Parameter] public string Style { get; set; }

    /// <summary>
    /// 取得或设置条形码组件条码值。
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <summary>
    /// 取得或设置条形码组件条码对象，参考Barcode.js配置。
    /// </summary>
    [Parameter] public object Option { get; set; }

    /// <summary>
    /// 呈现条形码组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Canvas().Id(id).Class(Style).Close();
    }

    /// <summary>
    /// 条形码组件呈现后，调用Barcode.js绘制条形码。
    /// </summary>
    /// <param name="firstRender">是否首次呈现。</param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender || Value != lastCode)
        {
            lastCode = Value;
            await JS.ShowBarcodeAsync(id, Value, Option);
        }
        await base.OnAfterRenderAsync(firstRender);
    }
}