namespace Known.Components;

/// <summary>
/// 上传附件组件类。
/// </summary>
public partial class KUpload
{
    private ElementReference container;
    private DotNetObjectReference<KUpload> invoker;
    private List<AttachInfo> sysFiles = [];
    private readonly List<FileDataInfo> files = [];
    private bool isAdding;
    private bool showFullscreen = false;
    private AttachInfo currentFullscreenImage;

    private bool IsMultiple => MultiFile || Directory;
    private bool IsReadOnly => ReadOnly || AntForm?.IsView == true;

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
    /// 取得或设置是否是图片组件。
    /// </summary>
    [Parameter] public bool IsImage { get; set; }

    /// <summary>
    /// 取得或设置是否显示删除按钮，默认显示。
    /// </summary>
    [Parameter] public bool IsDelete { get; set; } = true;

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
    /// 取得或设置上传组件是否压缩图片，默认不压缩。
    /// </summary>
    [Parameter] public bool IsCompress { get; set; }

    /// <summary>
    /// 取得或设置上传组件允许最大上传的文件大小，单位M。
    /// </summary>
    [Parameter] public int? MaxSize { get; set; }

    /// <summary>
    /// 取得或设置上传组件一次最大上传的文件数量，默认10。
    /// </summary>
    [Parameter] public int MaxCount { get; set; } = 10;

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
    /// 取得是否有附件。
    /// </summary>
    public bool HasFile => sysFiles != null && sysFiles.Count > 0 || files.Count > 0;

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

    /// <summary>
    /// JS调用粘贴图片。
    /// </summary>
    /// <param name="base64Image">图片数据。</param>
    /// <returns></returns>
    [JSInvokable]
    public async Task ReceivePastedImage(string base64Image)
    {
        // 将 base64 字符串转换为字节数组
        var imageData = Convert.FromBase64String(base64Image);
        var fileName = $"screenshot-{DateTime.Now:yyyyMMddHHmmss}.png";
        var file = new BrowserFile(imageData, fileName, "image/png");
        var isChange = await OnAddFileAsync(file, false);
        if (isChange)
        {
            await StateChangedAsync();
            if (OnFilesChanged != null)
                await OnFilesChanged.Invoke(files);
        }
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();
        invoker = DotNetObjectReference.Create(this);
        if (IsImage)
            Accept = "image/*";
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            await JS.InvokeAsync("KUtils.setupPasteListener", invoker, container);
            await RefreshAsync();
        }
    }

    /// <inheritdoc />
    protected override Task OnDisposeAsync()
    {
        invoker?.Dispose();
        return base.OnDisposeAsync();
    }

    private async Task OnPasteFileAsync()
    {
        await JS.InvokeAsync("KUtils.checkClipboardPermission");
    }

    private async Task OnInputFileChangedAsync(InputFileChangeEventArgs e)
    {
        isAdding = true;
        var isChange = false;
        if (MultiFile || Directory)
        {
            foreach (var item in e.GetMultipleFiles(MaxCount))
            {
                isChange = await OnAddFileAsync(item, IsCompress);
            }
        }
        else
        {
            isChange = await OnAddFileAsync(e.File, IsCompress);
        }
        isAdding = false;
        if (isChange)
        {
            await StateChangedAsync();
            if (OnFilesChanged != null)
                await OnFilesChanged.Invoke(files);
        }
    }

    private async Task<bool> OnAddFileAsync(IBrowserFile item, bool isCompress)
    {
        if (files.Exists(f => f.Name == item.Name))
            return false;

        var file = await item.CreateFileAsync(MaxSize, isCompress, CompressSize);
        if (file.MaxSize != null)
        {
            UI.Error(Language[Language.TipUploadMaxSize].Replace("{size}", $"{file.MaxSize}"));
            return false;
        }

        if (IsImage)
        {
            var image = await item.RequestImageFileAsync(item.ContentType, 60, 60);
            var info = await image.ReadFileAsync();
            file.Thumbnails = info.Bytes;
        }

        files.Add(file);
        sysFiles ??= [];
        sysFiles.Add(new AttachInfo
        {
            Id = "",
            Name = item.Name,
            OriginalData = file.Bytes,
            ThumbnailData = file.Thumbnails
        });
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

        var message = Language[Language.TipConfirmDelete].Replace("{name}", item.Name);
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

    private void OnShowFile(AttachInfo item)
    {
        UI.PreviewFile([item]);
    }

    private static string GetImageSrc(byte[] imageData)
    {
        if (imageData == null || imageData.Length == 0)
            return string.Empty;

        return $"data:image/jpeg;base64,{Convert.ToBase64String(imageData)}";
    }

    private void ShowFullscreen(AttachInfo image)
    {
        currentFullscreenImage = image;
        showFullscreen = true;
    }

    private void CloseFullscreen()
    {
        showFullscreen = false;
        currentFullscreenImage = null;
    }
}