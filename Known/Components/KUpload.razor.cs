namespace Known.Components;

/// <summary>
/// 上传附件组件类。
/// </summary>
public partial class KUpload
{
    private List<AttachInfo> sysFiles = [];
    private readonly List<FileDataInfo> files = [];

    private bool IsMultiple => MultiFile || Directory;

    [CascadingParameter] private IComContainer AntForm { get; set; }

    /// <summary>
    /// 取得或设置上传组件附件属性列表。
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object> InputAttributes { get; set; }

    /// <summary>
    /// 取得或设置是否支持拖拽。
    /// </summary>
    [Parameter] public bool IsDrag { get; set; }

    /// <summary>
    /// 取得或设置是否显示上传按钮。
    /// </summary>
    [Parameter] public bool IsButton { get; set; }

    /// <summary>
    /// 取得或设置上传组件关联字段值。
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <summary>
    /// 取得或设置上传文件提示信息。
    /// </summary>
    [Parameter] public string HintText { get; set; }

    /// <summary>
    /// 取得或设置上传组件关联的文件类型，例如：.jpg, .png, .doc。
    /// </summary>
    [Parameter] public string Accept { get; set; }

    /// <summary>
    /// 取得或设置上传文件的模板URL。
    /// </summary>
    [Parameter] public string TemplateUrl { get; set; }

    /// <summary>
    /// 取得或设置上传组件是否支持多文件上传。
    /// </summary>
    [Parameter] public bool MultiFile { get; set; }

    /// <summary>
    /// 取得或设置上传组件是否支持文件夹上传。
    /// </summary>
    [Parameter] public bool Directory { get; set; }

    /// <summary>
    /// 取得或设置上传组件是否压缩图片，默认压缩。
    /// </summary>
    [Parameter] public bool IsCompress { get; set; } = true;

    /// <summary>
    /// 取得或设置上传组件一次最大上传的文件数量，默认10。
    /// </summary>
    [Parameter] public int MaxFileCount { get; set; } = 10;

    /// <summary>
    /// 取得或设置上传组件压缩图片大小，默认1920*1080。
    /// </summary>
    [Parameter] public Size CompressSize { get; set; } = new Size(1920, 1080);

    /// <summary>
    /// 取得或设置上传组件文件改变事件委托。
    /// </summary>
    [Parameter] public Func<List<FileDataInfo>, Task> OnFilesChanged { get; set; }

    /// <summary>
    /// 取得或设置上传组件获取文件列表委托。
    /// </summary>
    [Parameter] public Func<string, Task<List<AttachInfo>>> OnLoad { get; set; }

    /// <summary>
    /// 取得或设置上传组件删除文件委托。
    /// </summary>
    [Parameter] public Func<AttachInfo, Task> OnDelete { get; set; }

    /// <summary>
    /// 清空文件列表。
    /// </summary>
    public void Clear()
    {
        files.Clear();
        sysFiles.Clear();
        StateChanged();
    }

    /// <inheritdoc />
    public override async Task RefreshAsync()
    {
        if (string.IsNullOrWhiteSpace(Value))
            return;

        if (OnLoad != null)
            sysFiles = await OnLoad.Invoke(Value);
        else
            sysFiles = await Admin.GetFilesAsync(Value);
        await StateChangedAsync();
    }

    /// <summary>
    /// 异步设置上传组件关联字段值。
    /// </summary>
    /// <param name="value">附件字段值。</param>
    public async Task SetValueAsync(string value)
    {
        Value = value;
        await RefreshAsync();
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (AntForm != null)
            ReadOnly = AntForm.IsView;
        base.OnInitialized();
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
            await RefreshAsync();
    }

    private async Task OnInputFileChangedAsync(InputFileChangeEventArgs e)
    {
        var isChange = false;
        if (MultiFile || Directory)
        {
            foreach (var item in e.GetMultipleFiles(MaxFileCount))
            {
                isChange = await OnAddFileAsync(item);
            }
        }
        else
        {
            isChange = await OnAddFileAsync(e.File);
        }

        if (isChange)
        {
            await StateChangedAsync();
            if (OnFilesChanged != null)
                await OnFilesChanged.Invoke(files);
        }
    }

    private async Task<bool> OnAddFileAsync(IBrowserFile item)
    {
        if (files.Exists(f => f.Name == item.Name))
            return false;

        var file = await item.CreateFileAsync(IsCompress, CompressSize);
        files.Add(file);
        sysFiles ??= [];
        sysFiles.Add(new AttachInfo { Id = "", Name = item.Name });
        return true;
    }

    private async Task OnDeleteFileAsync(AttachInfo item)
    {
        if (string.IsNullOrWhiteSpace(item.Id))
        {
            var file = files.FirstOrDefault(f => f.Name == item.Name);
            files.Remove(file);
            sysFiles?.Remove(item);
            if (OnFilesChanged != null)
                await OnFilesChanged.Invoke(files);
            return;
        }

        var message = Language["Tip.ConfirmDelete"].Replace("{name}", item.Name);
        UI.Confirm(message, async () =>
        {
            if (OnDelete != null)
                await OnDelete.Invoke(item);
            else
                await Admin.DeleteFileAsync(item);
            sysFiles?.Remove(item);
            await StateChangedAsync();
        });
    }
}