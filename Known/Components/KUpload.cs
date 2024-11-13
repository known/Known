namespace Known.Components;

/// <summary>
/// 上传组件类。
/// </summary>
public class KUpload : BaseComponent
{
    private List<AttachInfo> sysFiles;
    private readonly List<FileDataInfo> files = [];

    /// <summary>
    /// 取得或设置是否显示上传按钮。
    /// </summary>
    [Parameter] public bool IsButton { get; set; }

    /// <summary>
    /// 取得或设置上传组件关联字段值。
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <summary>
    /// 取得或设置上传组件是否支持多文件上传。
    /// </summary>
    [Parameter] public bool MultiFile { get; set; }

    /// <summary>
    /// 取得或设置上传组件是否支持文件夹上传。
    /// </summary>
    [Parameter] public bool Directory { get; set; }

    /// <summary>
    /// 取得或设置上传组件文件改变事件委托。
    /// </summary>
    [Parameter] public Func<List<FileDataInfo>, Task> OnFilesChanged { get; set; }

    /// <summary>
    /// 刷新上传组件内容。
    /// </summary>
    /// <returns></returns>
    public override async Task RefreshAsync()
    {
        if (string.IsNullOrWhiteSpace(Value))
            return;

        sysFiles = await System.GetFilesAsync(Value);
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
    /// 上传组件呈现后，调用后端接口加载附件列表。
    /// </summary>
    /// <param name="firstRender">是否首次呈现。</param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            if (!string.IsNullOrWhiteSpace(Value))
            {
                sysFiles = await System.GetFilesAsync(Value);
                await StateChangedAsync();
            }
        }
    }

    /// <summary>
    /// 呈现上传组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
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
               .Set(c => c.OnChange, this.Callback<InputFileChangeEventArgs>(OnInputFileChangedAsync))
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
                        builder.Span("kui-link kui-danger", Language.Delete, this.Callback<MouseEventArgs>(e => OnDeleteFile(item)));
                    builder.FileLink(item);
                });
            }
        });
    }

    private async Task OnInputFileChangedAsync(InputFileChangeEventArgs e)
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
            await OnFilesChanged?.Invoke(files);
        }
    }

    private async Task<bool> OnAddFileAsync(IBrowserFile item)
    {
        if (files.Exists(f => f.Name == item.Name))
            return false;

        var file = await item.CreateFileAsync();
        files.Add(file);
        sysFiles ??= [];
        sysFiles.Add(new AttachInfo { Id = "", Name = item.Name });
        return true;
    }

    private void OnDeleteFile(AttachInfo item)
    {
        if (string.IsNullOrWhiteSpace(item.Id))
        {
            var file = files.FirstOrDefault(f => f.Name == item.Name);
            files.Remove(file);
            sysFiles?.Remove(item);
            OnFilesChanged?.Invoke(files);
            return;
        }

        var message = Language["Tip.ConfirmDelete"].Replace("{name}", item.Name);
        UI.Confirm(message, async () =>
        {
            await System.DeleteFileAsync(item);
            sysFiles?.Remove(item);
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
        OnFilesChanged = files =>
        {
            Model.Form.Files[Id] = files;
            Model.Value = Id;
            return Task.CompletedTask;
        };
        await base.OnInitAsync();
    }
}