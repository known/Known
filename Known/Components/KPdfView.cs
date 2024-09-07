namespace Known.Components;

/// <summary>
/// PDF查看组件类，配置参考pdfobject.js。
/// </summary>
public class KPdfView : BaseComponent
{
    /// <summary>
    /// 取得或设置PDF组件样式字符串。
    /// </summary>
    [Parameter] public string Style { get; set; }

    /// <summary>
    /// 取得或设置PDF文件流。
    /// </summary>
    [Parameter] public Stream Stream { get; set; }

    /// <summary>
    /// 呈现PDF查看组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div().Id(Id).Class(Style).Close();
    }

    /// <summary>
    /// PDF查看组件呈现后，调用pdfobject.js显示PDF文件。
    /// </summary>
    /// <param name="firstRender">是否首次呈现。</param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await JS.ShowPdfAsync(Id, Stream);
        await base.OnAfterRenderAsync(firstRender);
    }
}