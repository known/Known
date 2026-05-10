namespace Known.Components;

/// <summary>
/// 附件预览组件类。
/// </summary>
public partial class KFileView
{
    private AttachInfo current;
    private string currentImageUrl;
    private bool showPdf;
    private string PreviewUrl => Config.App.Type == AppType.Web ? current?.FileUrl?.OriginalUrl : currentImageUrl;

    /// <summary>
    /// 取得或设置附件列表。
    /// </summary>
    [Parameter] public List<AttachInfo> Items { get; set; }

    /// <summary>
    /// 取得或设置附件字段值。
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <inheritdoc />
    protected override async Task OnRenderAsync(bool firstRender)
    {
        await base.OnRenderAsync(firstRender);
        if (firstRender)
        {
            if (!string.IsNullOrWhiteSpace(Value))
                Items = await Admin.GetFilesAsync(Value);
            var item = Items?.FirstOrDefault();
            await OnFileClickAsync(item);
            if (showPdf)
                StateChanged();
        }
        if (showPdf)
        {
            showPdf = false;
            if (Config.App.Type == AppType.Web)
            {
                await JS.ShowPdfAsync($"pdfView-{current?.Id}", current?.FileUrl?.OriginalUrl);
            }
            else
            {
                var path = current.GetLocalPath();
                if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                {
                    await using var stream = File.OpenRead(path);
                    await JS.ShowPdfAsync($"pdfView-{current?.Id}", stream);
                }
            }
        }
    }

    private async Task OnFileClickAsync(AttachInfo item)
    {
        current = item;
        currentImageUrl = await item.GetImageUrlAsync();
        if (item?.SourceName?.EndsWith(".pdf") == true)
            showPdf = true;
    }

    private Task OnDownload(MouseEventArgs args)
    {
        return App?.DownloadAsync(current);
    }
}