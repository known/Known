namespace Known.Components;

/// <summary>
/// 附件预览组件类。
/// </summary>
public partial class KFileView
{
    private AttachInfo current;
    private bool showPdf;

    /// <summary>
    /// 取得或设置附件列表。
    /// </summary>
    [Parameter] public List<AttachInfo> Items { get; set; }

    /// <summary>
    /// 取得或设置附件字段值。
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            if (!string.IsNullOrWhiteSpace(Value))
                Items = await Admin.GetFilesAsync(Value);
            var item = Items?.FirstOrDefault();
            OnFileClick(item);
            if (showPdf)
                StateChanged();
        }
        if (showPdf)
        {
            showPdf = false;
            await JS.ShowPdfAsync($"pdfView-{current?.Id}", current?.FileUrl?.OriginalUrl);
        }
    }

    private void OnFileClick(AttachInfo item)
    {
        current = item;
        if (item?.SourceName?.EndsWith(".pdf") == true)
            showPdf = true;
    }

    private Task OnDownload(MouseEventArgs args)
    {
        return App?.DownloadAsync(current);
    }
}