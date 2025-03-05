﻿namespace Known.Internals;

class Importer : BaseComponent
{
    private bool isFinished;
    private string fileInfo;
    private string error;
    private string message;
    private FileDataInfo file;
    private ImportFormInfo Model;

    private string ErrorMessage => Language["Import.Error"];

    [Parameter] public ImportInfo Info { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();

        var type = Info.EntityType;
        var id = $"{type.Name}Import";
        if (!string.IsNullOrWhiteSpace(Info.Param))
            id += $"_{Info.Param}";
        if (Info.IsDictionary)
            id = $"AutoImport_{Info.PageId}";
        Model = await Admin.GetImportAsync(id);
        Model.Name = Info.PageName;
        Model.BizName = $"导入{Info.PageName}";

        isFinished = Model.IsFinished;
        error = Model.Error;
        message = Model.Message;
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-import", () =>
        {
            builder.Div("kui-primary", Language["Import.Tips"]);
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
                        Label = Language["Import.IsAsync"],
                        Value = Model?.IsAsync == true,
                        ValueChanged = this.Callback<bool>(v => Model.IsAsync = v)
                    });
                });
            });
            builder.Div(() =>
            {
                builder.Link(Language["Import.Download"], this.Callback(OnDownloadTemplateAsync));
                if (!string.IsNullOrWhiteSpace(error))
                    builder.Span().Class("kui-link kui-danger").OnClick(this.Callback(OnErrorMessage)).Markup(ErrorMessage);
                builder.Span("size", fileInfo);
            });
            var style = string.IsNullOrWhiteSpace(error) ? "kui-primary" : "kui-danger";
            builder.Div($"kui-import-message {style}", message);
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

        fileInfo = $"{Language["Import.Size"]}{e.File.Size / 1024}KB";
        file = await e.File.ReadFileAsync();
    }

    private async Task OnImportAsync(MouseEventArgs e)
    {
        if (file == null)
        {
            UI.Error(Language["Import.SelectFile"]);
            return;
        }

        message = Language["Import.Importing"];
        isFinished = false;

        var info = new UploadInfo<ImportFormInfo>(Model);
        info.Files["Upload"] = [file];
        var result = await Admin.ImportFilesAsync(info);
        if (!result.IsValid)
        {
            error = result.Message;
            message = Language["Import.TaskFailed"];
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
                UI.Error(Language["Import.FileNotExists"]);
                return;
            }

            await JS.DownloadFileAsync($"{Language["Import.Template"]}_{Model.Name}.xlsx", bytes);
        });
    }
}