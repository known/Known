namespace Known.Components;

/// <summary>
/// PDF查看组件类，配置参考pdfobject.js。
/// </summary>
public partial class KPdfView
{
    /// <summary>
    /// 取得或设置PDF组件选项，选项参考pdfobject.js。
    /// </summary>
    [Parameter] public object Option { get; set; }

    /// <summary>
    /// 异步显示PDF文件。
    /// </summary>
    /// <param name="stream">PDF文件流。</param>
    /// <returns></returns>
    public async Task ShowAsync(Stream stream)
    {
        if (stream == null || !Visible)
            return;

        await JS.ShowPdfAsync(Id, stream, Option);
    }

    /// <summary>
    /// 异步显示PDF文件。
    /// </summary>
    /// <param name="pdfFile">PDF文件名或URL。</param>
    /// <returns></returns>
    public async Task ShowAsync(string pdfFile)
    {
        if (string.IsNullOrWhiteSpace(pdfFile) || !Visible)
            return;

        if (IsServerMode)
        {
            if (File.Exists(pdfFile))
            {
                var bytes = await File.ReadAllBytesAsync(pdfFile);
                var stream = new MemoryStream(bytes);
                await ShowAsync(stream);
                return;
            }
        }

        await JS.ShowPdfAsync(Id, pdfFile, Option);
    }
}