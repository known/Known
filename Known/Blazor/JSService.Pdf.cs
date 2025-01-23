namespace Known.Blazor;

public partial class JSService
{
    /// <summary>
    /// 异步显示PDF文件。
    /// </summary>
    /// <param name="id">PDF前端控件ID。</param>
    /// <param name="url">PDF文件URL。</param>
    /// <param name="option">PDF选项。</param>
    /// <returns></returns>
    public async Task ShowPdfAsync(string id, string url, object option = null)
    {
        if (string.IsNullOrWhiteSpace(url))
            return;

        option ??= new { forceIframe = true };
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("KBlazor.showPdfByUrl", id, url, option);
    }

    /// <summary>
    /// 异步显示PDF文件。
    /// </summary>
    /// <param name="id">PDF前端控件ID。</param>
    /// <param name="stream">PDF文件流。</param>
    /// <param name="option">PDF选项。</param>
    /// <returns></returns>
    public async Task ShowPdfAsync(string id, Stream stream, object option = null)
    {
        if (stream == null)
            return;

        option ??= new { forceIframe = true };
        var module = await moduleTask.Value;
        using var streamRef = new DotNetStreamReference(stream);
        await module.InvokeVoidAsync("KBlazor.showPdfByStream", id, streamRef, option);
    }
}