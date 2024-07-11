namespace Known.Components;

public class KUpload : BaseComponent
{
    private IFileService fileService;
    private List<SysFile> sysFiles;
    private readonly List<FileDataInfo> files = [];

    [Parameter] public bool IsButton { get; set; }
    [Parameter] public bool OpenFile { get; set; }
    [Parameter] public string Value { get; set; }
    [Parameter] public bool MultiFile { get; set; }
    [Parameter] public bool Directory { get; set; }
    [Parameter] public Action<List<FileDataInfo>> OnFilesChanged { get; set; }

    public async Task RefreshAsync()
    {
        if (string.IsNullOrWhiteSpace(Value))
            return;

        sysFiles = await fileService.GetFilesAsync(Value);
        await StateChangedAsync();
    }

    public async void SetValue(string value)
    {
        Value = value;
        await RefreshAsync();
    }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        fileService = await CreateServiceAsync<IFileService>();
        if (!string.IsNullOrWhiteSpace(Value))
            sysFiles = await fileService.GetFilesAsync(Value);
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (IsButton)
        {
            builder.Div("kui-upload", () =>
            {
                if (!ReadOnly)
                {
                    builder.Div("kui-button", () =>
                    {
                        builder.Icon("upload");
                        builder.Text(Language.Upload);
                        BuildInputFile(builder);
                    });
                }
                BuildFiles(builder);
            });
        }
        else
        {
            if (!ReadOnly)
                BuildInputFile(builder);
            BuildFiles(builder);
        }
    }

    private void BuildInputFile(RenderTreeBuilder builder)
    {
        builder.Component<InputFile>()
               .Add("multiple", MultiFile || Directory)
               .Add("webkitdirectory", Directory)
               .Set(c => c.OnChange, this.Callback<InputFileChangeEventArgs>(OnInputFileChanged))
               .Build();
    }

    private void BuildFiles(RenderTreeBuilder builder)
    {
        if (sysFiles == null || sysFiles.Count == 0)
            return;

        builder.Div("kui-form-files", () =>
        {
            foreach (var item in sysFiles)
            {
                var url = item.FileUrl;
                builder.Div(() =>
                {
                    if (!ReadOnly)
                        builder.Span("kui-link danger", Language.Delete, this.Callback<MouseEventArgs>(e => OnDeleteFile(item)));
                    builder.OpenFile(item.Name, item.FileUrl, !OpenFile);
                });
            }
        });
    }

    private async void OnInputFileChanged(InputFileChangeEventArgs e)
    {
        var isChange = false;
        if (MultiFile || Directory)
        {
            foreach (var item in e.GetMultipleFiles())
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
            OnFilesChanged?.Invoke(files);
        }
    }

    private async Task<bool> OnAddFileAsync(IBrowserFile item)
    {
        if (files.Exists(f => f.Name == item.Name))
            return false;

        var file = await item.CreateFileAsync();
        files.Add(file);
        sysFiles.Add(new SysFile { Id = "", Name = item.Name });
        return true;
    }

    private void OnDeleteFile(SysFile item)
    {
        if (string.IsNullOrWhiteSpace(item.Id))
        {
            var file = files.FirstOrDefault(f => f.Name == item.Name);
            files.Remove(file);
            sysFiles.Remove(item);
            OnFilesChanged?.Invoke(files);
            return;
        }

        var message = Language["Tip.ConfirmDelete"].Replace("{name}", item.Name);
        UI.Confirm(message, async () =>
        {
            await fileService.DeleteFileAsync(item);
            sysFiles.Remove(item);
            await StateChangedAsync();
        });
    }
}

class KUploadField<TItem> : KUpload where TItem : class, new()
{
    [Parameter] public FieldModel<TItem> Model { get; set; }

    protected override async Task OnInitAsync()
    {
        Id = Model.Column.Id;
        ReadOnly = Model.Form.IsView;
        Value = Model.Value?.ToString();
        MultiFile = Model.Column.MultiFile;
        OnFilesChanged = files => Model.Form.Files[Id] = files;
        await base.OnInitAsync();
    }
}