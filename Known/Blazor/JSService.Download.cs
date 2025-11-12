namespace Known.Blazor;

public partial class JSService
{
    /// <summary>
    /// 异步下载文件。
    /// </summary>
    /// <param name="fileName">文件名。</param>
    /// <param name="url">下载地址。</param>
    /// <returns></returns>
    public Task DownloadFileAsync(string fileName, string url)
    {
        return InvokeAsync("KBlazor.downloadFileByUrl", fileName, url);
    }

    /// <summary>
    /// 异步下载文件流。
    /// </summary>
    /// <param name="fileName">文件名。</param>
    /// <param name="stream">文件流。</param>
    /// <returns></returns>
    public async Task DownloadFileAsync(string fileName, Stream stream)
    {
        using var streamRef = new DotNetStreamReference(stream);
        await InvokeAsync("KBlazor.downloadFileByStream", fileName, streamRef);
    }
}