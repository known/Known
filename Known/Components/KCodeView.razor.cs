namespace Known.Components;

/// <summary>
/// 代码查看器组件类。
/// </summary>
public partial class KCodeView
{
    private ReloadContainer container;
    private bool shouldHighlight;
    private string originalCode;

    /// <summary>
    /// 取得或设置样式类。
    /// </summary>
    [Parameter] public string Class { get; set; }

    /// <summary>
    /// 取得或设置代码。
    /// </summary>
    [Parameter] public string Code { get; set; }

    /// <summary>
    /// 取得或设置代码语言。
    /// </summary>
    [Parameter] public string Lang { get; set; }

    /// <summary>
    /// 取得或设置代码文件名。
    /// </summary>
    [Parameter] public string FileName { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        originalCode = Code;
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender || shouldHighlight)
        {
            shouldHighlight = false;
            await JSRuntime.HighlightAllAsync();
        }
    }

    /// <summary>
    /// 设置代码。
    /// </summary>
    /// <param name="code">代码。</param>
    public void SetCode(string code)
    {
        if (originalCode == code)
            return;

        originalCode = code;
        Code = code;
        shouldHighlight = true;
        container?.ReloadPage();
    }

    private void OnCopy(MouseEventArgs args)
    {
        JSRuntime.CopyTextAsync(Code);
        UI.Success("复制成功！");
    }

    private Task OnDownload(MouseEventArgs args)
    {
        var bytes = Encoding.UTF8.GetBytes(Code);
        var info = new FileDataInfo(FileName, bytes);
        return JS.DownloadFileAsync(info);
    }
}