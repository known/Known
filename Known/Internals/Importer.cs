namespace Known.Internals;

class Importer : BaseComponent
{
    private bool isFinished;
    private string fileInfo;
    private string error;
    private string message;
    private FileDataInfo file;
    private ImportFormInfo Model;

    private string ErrorMessage => Language[Language.ImportError];

    [Parameter] public ImportInfo Info { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();

        var type = Info.EntityType;
        var id = type.Name;
        if (!string.IsNullOrWhiteSpace(Info.Param))
            id += $"_{Info.Param}";
        if (Info.IsDictionary)
            id = $"{Config.AutoBizIdPrefix}_{Info.PageId}_{Info.PluginId}";
        Model = await Admin.GetImportAsync(id);
        Model.Name = Info.PageName;
        Model.BizName = Language[Language.ImportTitle].Replace("{name}", Info.PageName);

        isFinished = Model.IsFinished;
        error = Model.Error;
        message = Model.Message;
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-import", () =>
        {
            builder.Div("ant-btn-link", Language[Language.ImportTips]);
            builder.Div("item", () =>
            {
                BuildInputFile(builder);
                if (isFinished)
                    builder.Button(Language.Import, this.Callback<MouseEventArgs>(OnImportAsync));
                builder.Div("async", () =>
                {
                    builder.CheckBox(new InputModel<bool>
                    {
                        Disabled = !isFinished,
                        Label = Language[Language.ImportIsAsync],
                        Value = Model?.IsAsync == true,
                        ValueChanged = this.Callback<bool>(v => Model.IsAsync = v)
                    });
                });
            });
            builder.Div(() =>
            {
                builder.Link(Language[Language.ImportDownload], this.Callback(OnDownloadTemplateAsync));
                if (!string.IsNullOrWhiteSpace(error))
                    builder.Span().Class("kui-link kui-danger").OnClick(this.Callback(OnErrorMessage)).Markup(ErrorMessage);
                builder.Span("size", fileInfo);
            });
            var style = string.IsNullOrWhiteSpace(error) ? "ant-btn-link" : "kui-danger";
            builder.Div($"kui-import-message {style}", Language[message]);
        });
    }

    private void OnErrorMessage()
    {
        UI.ShowDialog(new DialogModel
        {
            Title = ErrorMessage,
            Content = builder => builder.Markup(error)
        });
    }

    private void BuildInputFile(RenderTreeBuilder builder)
    {
        builder.OpenComponent<InputFile>(0);
        builder.AddAttribute(1, "accept", "text/plain,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        builder.AddAttribute(2, "disabled", !isFinished);
        builder.AddAttribute(3, "OnChange", this.Callback<InputFileChangeEventArgs>(OnInputFilesChanged));
        builder.CloseComponent();
    }

    private async Task OnInputFilesChanged(InputFileChangeEventArgs e)
    {
        if (e.File == null || e.File.Size == 0)
            return;

        fileInfo = $"{Language[Language.ImportSize]}{e.File.Size / 1024}KB";
        file = await e.File.ReadFileAsync();
    }

    private async Task OnImportAsync(MouseEventArgs e)
    {
        if (file == null)
        {
            UI.Error(Language.ImportSelectFile);
            return;
        }

        message = Language[Language.ImportImporting];
        isFinished = false;

        var info = new UploadInfo<ImportFormInfo>(Model);
        info.Files["Upload"] = [file];
        var result = await Admin.ImportFilesAsync(info);
        if (!result.IsValid)
        {
            error = result.Message;
            message = Language[Language.ImportFailed];
            isFinished = true;
            await StateChangedAsync();
            return;
        }

        if (Model.IsAsync)
        {
            message = result.Message;
            isFinished = false;
        }
        else
        {
            Info?.OnSuccess?.Invoke();
        }
    }

    private Task OnDownloadTemplateAsync()
    {
        return App?.DownloadAsync(async () =>
        {
            var bytes = await Admin.GetImportRuleAsync(Model.BizId);
            if (bytes == null || bytes.Length == 0)
            {
                UI.Error(Language.ImportFileNotExists);
                return;
            }

            await JS.DownloadFileAsync($"{Language[Language.ImportTemplate]}_{Model.Name}.xlsx", bytes);
        });
    }
}