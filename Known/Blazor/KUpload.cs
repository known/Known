namespace Known.Blazor;

public class KUpload : BaseComponent
{
    private List<SysFile> sysFiles;
    private readonly List<IBrowserFile> files = [];

    [Parameter] public bool IsButton { get; set; }
    [Parameter] public bool OpenFile { get; set; }
    [Parameter] public string Value { get; set; }
    [Parameter] public bool MultiFile { get; set; }
    [Parameter] public Action<List<IBrowserFile>> OnFilesChanged { get; set; }

    public async Task RefreshAsync()
    {
        sysFiles = await Platform.GetFilesAsync(Value);
        StateChanged();
    }

    public async void SetValue(string value)
    {
        Value = value;
        await RefreshAsync();
    }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        sysFiles = await Platform.GetFilesAsync(Value);
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
                        UI.Icon(builder, "upload");
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
               .Add("multiple", MultiFile)
               .Add("webkitdirectory", MultiFile)
               .Set(c => c.OnChange, this.Callback<InputFileChangeEventArgs>(OnInputFileChanged))
               .Build();
    }

    private void BuildFiles(RenderTreeBuilder builder)
    {
        if (sysFiles != null && sysFiles.Count > 0)
        {
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
    }

    private void OnInputFileChanged(InputFileChangeEventArgs e)
    {
        if (MultiFile)
            files.AddRange(e.GetMultipleFiles());
        else
            files.Add(e.File);

        foreach (var item in files)
        {
            if (!sysFiles.Exists(f => f.Name == item.Name))
                sysFiles.Add(new SysFile { Id = "", Name = item.Name });
        }

        OnFilesChanged?.Invoke(files);
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
            await Platform.File.DeleteFileAsync(item);
            sysFiles.Remove(item);
            StateChanged();
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