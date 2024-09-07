namespace Known.Components;

/// <summary>
/// 二维码组件类，配置参考qrcode.js。
/// </summary>
public class KQRCode : BaseComponent
{
    private readonly string id;

    /// <summary>
    /// 构造函数，创建一个二维码组件类的实例。
    /// </summary>
    public KQRCode()
    {
        id = Utils.GetGuid();
        id = $"qr-{id}";
    }

    /// <summary>
    /// 取得或设置组件样式字符串。
    /// </summary>
    [Parameter] public string Style { get; set; }

    /// <summary>
    /// 取得或设置二维码配置对象。
    /// </summary>
    [Parameter] public object Option { get; set; }

    /// <summary>
    /// 呈现二维码组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div().Id(id).Class(Style).Close();
    }

    /// <summary>
    /// 二维码组件呈现后，调用qrcode.js显示二维码。
    /// </summary>
    /// <param name="firstRender">是否首次呈现。</param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await JS.ShowQRCodeAsync(id, Option);
        await base.OnAfterRenderAsync(firstRender);
    }
}