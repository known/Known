using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Known.Blazor;

class Importer : BaseComponent
{
    private bool isFinished;
    private string fileInfo;
    private string error;
    private string message;
    private IAttachFile attach;

    private string ErrorMessage => Language["Import.Error"];

    [Parameter] public ImportFormInfo Model { get; set; }
    [Parameter] public Action OnSuccess { get; set; }

    protected override Task OnInitializedAsync()
    {
        isFinished = Model.IsFinished;
        error = Model.Error;
        message = Model.Message;
        return base.OnInitializedAsync();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("kui-form-import", () =>
        {
            builder.Div("danger", Language["Import.Tips"]);
            builder.Div("item", () =>
            {
                BuildInputFile(builder);
                if (isFinished)
                {
                    UI.Button(builder, new ActionInfo(Context, "Import", ""), this.Callback<MouseEventArgs>(OnImportAsync));
                }
            });
            builder.Div(() =>
            {
                builder.Link(Language["Import.Download"], this.Callback(OnDownloadTemplateAsync));
                if (!string.IsNullOrWhiteSpace(error))
                    builder.Link(ErrorMessage, this.Callback(OnErrorMessage));
                builder.Span("size", fileInfo);
            });
            var style = string.IsNullOrWhiteSpace(error) ? "primary" : "danger";
            builder.Div($"message {style}", message);
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

    private void OnInputFilesChanged(InputFileChangeEventArgs e)
    {
        var file = e.File;
        if (file == null || file.Size == 0)
            return;

        fileInfo = $"{Language["Import.Size"]}{file.Size / 1024}KB";
        attach = new BlazorAttachFile(file);
    }

    private async void OnImportAsync(MouseEventArgs e)
    {
        if (attach == null)
        {
            UI.Error(Language["Import.SelectFile"]);
            return;
        }

        message = Language["Import.Importing"];
        isFinished = false;

        var info = new UploadInfo<ImportFormInfo>(Model);
        info.Files["Upload"] = [attach];
        var result = await Platform.File.UploadFilesAsync(info);
        if (!result.IsValid)
        {
            message = result.Message;
            isFinished = true;
            return;
        }

        if (Model.IsAsync)
        {
            message = result.Message;
            isFinished = false;
        }
        else
        {
            OnSuccess?.Invoke();
        }
    }

    private async Task OnDownloadTemplateAsync()
    {
        var bytes = await Platform.File.GetImportRuleAsync(Model.BizId);
        if (bytes != null && bytes.Length > 0)
        {
            var stream = new MemoryStream(bytes);
            await JS.DownloadFileAsync($"{Language["Import.Template"]}_{Model.Name}.xlsx", stream);
        }
    }
}