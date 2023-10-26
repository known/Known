using Known.Extensions;

namespace Known.Razor;

public class KUpload : Field
{
    public KUpload()
    {
        Style = "upload";
    }

    [Parameter] public bool IsMobile { get; set; }
    [Parameter] public bool IsButton { get; set; }
    [Parameter] public string ButtonText { get; set; }
    [Parameter] public bool CanDelete { get; set; }
    [Parameter] public bool IsMultiple { get; set; }
    [Parameter] public string Accept { get; set; }
    [Parameter] public Action<List<IBrowserFile>> OnFilesChanged { get; set; }
    [Parameter] public List<SysFile> SysFiles { get; set; }
    public static long MaxLength { get; set; } = 1024 * 1024 * 50;
    public List<IBrowserFile> Files { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        if (!string.IsNullOrWhiteSpace(Value))
            SysFiles ??= await Platform.File.GetFilesAsync(Value);
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        UI.SetFormList();
        return base.OnAfterRenderAsync(firstRender);
    }

    public override bool Validate()
    {
        var isNotValid = (Files == null || Files.Count == 0) && Required;
        if (isNotValid && (SysFiles == null || SysFiles.Count == 0))
        {
            SetError(true);
            return false;
        }

        return true;
    }

    protected override void BuildText(RenderTreeBuilder builder) => BuildFileContent(builder);

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        if (IsMobile && Accept == Constants.MimeImage)
        {
            builder.Div("file-item add", attr =>
            {
                builder.Icon(attr => attr.Class("fa fa-camera"));
                BuildInputFile(builder);
            });
        }
        else
        {
            if (IsButton)
                BuildButtonFile(builder);
            else
                BuildInputFile(builder);
        }
        BuildMultiFileName(builder);
        BuildFileContent(builder);
    }

    private void BuildButtonFile(RenderTreeBuilder builder)
    {
        builder.Label("file-button button primary", attr =>
        {
            BuildInputFile(builder);
            builder.Span("file-text", attr =>
            {
                builder.Span(attr => attr.Class("fa fa-upload"));
                builder.Span(ButtonText);
            });
        });
    }

    private void BuildInputFile(RenderTreeBuilder builder)
    {
        builder.OpenComponent<InputFile>(0);
        builder.AddAttribute(1, "class", "upload");
        builder.AddAttribute(1, "disabled", !Enabled);
        builder.AddAttribute(1, nameof(InputFile.OnChange), Callback<InputFileChangeEventArgs>(OnInputFilesChanged));
        if (IsMultiple)
            builder.AddAttribute(1, "multiple", true);
        if (!string.IsNullOrWhiteSpace(Accept))
            builder.AddAttribute(1, "accept", Accept);
        builder.CloseComponent();
    }

    private void BuildMultiFileName(RenderTreeBuilder builder)
    {
        if (Files == null || Files.Count == 0)
            return;

        if (IsMobile && Accept == Constants.MimeImage)
        {
            foreach (var item in Files)
            {
                builder.Div("file-item", attr =>
                {
                    //var url = KRUtils.GetImageUrl(item.GetBytes());
                    //builder.Img(attr => attr.Src(url));
                    builder.Span(item.Name);
                });
            }
        }
        else if (Files.Count > 1)
        {
            builder.Div("form-files", attr =>
            {
                foreach (var item in Files)
                {
                    builder.Span(item.Name);
                }
            });
        }
    }

    private void BuildFileContent(RenderTreeBuilder builder)
    {
        if (SysFiles == null || SysFiles.Count == 0)
            return;

        if (IsMobile && Accept == Constants.MimeImage)
            BuildMobileFiles(builder);
        else
            BuildFormFiles(builder);
    }

    private void BuildMobileFiles(RenderTreeBuilder builder)
    {
        foreach (var item in SysFiles)
        {
            builder.Div("file-item", attr =>
            {
                builder.Img(attr => attr.Id(item.Id).Src(item.Url));
                if (CanDelete && !IsReadOnly)
                    builder.Span("close fa fa-times-circle", attr => attr.OnClick(Callback(e => OnDeleteFile(item))));
            });
        }
    }

    private void OnDeleteFile(SysFile item)
    {
        UI.Confirm($"确定要删除{item.Name}？", async () =>
        {
            await Platform.File.DeleteFileAsync(item);
            SysFiles.Remove(item);
            StateChanged();
        });
    }

    private void BuildFormFiles(RenderTreeBuilder builder)
    {
        builder.Div("form-files", attr =>
        {
            foreach (var item in SysFiles)
            {
                builder.Div("form-file", attr =>
                {
                    if (CanDelete && !IsReadOnly)
                        builder.Link("删除", Callback(e => OnDeleteFile(item)), "danger");

                    var url = item.FileUrl;
                    builder.Anchor(item.Name, url.OriginalUrl, url.FileName);
                });
            }
        });
    }

    private async void OnInputFilesChanged(InputFileChangeEventArgs e)
    {
        if (IsMultiple && e.FileCount > 20)
        {
            UI.Toast("一次最多上传20个文件！");
            return;
        }

        Files = new List<IBrowserFile>();
        if (IsMultiple)
        {
            var files = e.GetMultipleFiles(20);
            if (files == null || files.Count == 0)
                return;

            foreach (var file in files)
            {
                Files.Add(await GetFile(file));
            }
        }
        else
        {
            var file = e.File;
            if (file == null || file.Size == 0)
                return;

            Files.Add(await GetFile(file));
        }

        StateChanged();
        OnFilesChanged?.Invoke(Files);
    }

    private async Task<IBrowserFile> GetFile(IBrowserFile file)
    {
        if (Accept == Constants.MimeImage)
            return await file.RequestImageFileAsync(file.ContentType, 1920, 1080);

        return await Task.FromResult(file);
    }

    public static async Task<byte[]> GetBytes(IBrowserFile file)
    {
        using var stream = new MemoryStream();
        await file.OpenReadStream(MaxLength).CopyToAsync(stream);
        var bytes = stream.GetBuffer();
        return bytes;
    }
}