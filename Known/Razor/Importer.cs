using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Known.Razor;

class Importer : BaseComponent
{
    private bool isFinished;
    private string fileInfo;
    private string error;
    private string message;
    private IAttachFile attach;

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
        builder.Div("form-import", () =>
        {
            builder.Div("danger", "提示: 请上传单个txt或Excel格式附件！");
            builder.Div("item", () =>
            {
                BuildInputFile(builder);
                if (isFinished)
                {
                    UI.BuildButton(builder, new ButtonOption
                    {
                        Text = "导入",
                        OnClick = Callback<MouseEventArgs>(OnImportAsync)
                    });
                }
            });
            builder.Div(() =>
            {
                builder.Link("模板下载", Callback(OnDownloadTemplateAsync));
                if (!string.IsNullOrWhiteSpace(error))
                    builder.Link("错误信息", Callback(OnErrorMessage));
                builder.Span("size", fileInfo);
            });
            var style = string.IsNullOrWhiteSpace(error) ? "primary" : "danger";
            builder.Div($"message {style}", message);
        });
    }

    private void OnErrorMessage()
    {
        UI.ShowModal(new ModalOption
        {
            Title = "错误信息",
            Content = builder => builder.Markup(error)
        });
    }

    private void BuildInputFile(RenderTreeBuilder builder)
    {
        builder.OpenComponent<InputFile>(0);
        builder.AddAttribute(1, "accept", "text/plain,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        builder.AddAttribute(2, "disabled", !isFinished);
        builder.AddAttribute(3, "OnChange", Callback<InputFileChangeEventArgs>(OnInputFilesChanged));
        builder.CloseComponent();
    }

    private void OnInputFilesChanged(InputFileChangeEventArgs e)
    {
        var file = e.File;
        if (file == null || file.Size == 0)
            return;

        fileInfo = $"大小:{file.Size / 1024}KB";
        attach = new BlazorAttachFile(file);
    }

    private async void OnImportAsync(MouseEventArgs e)
    {
        if (attach == null)
        {
            UI.Error("请选择导入文件！");
            return;
        }

        message = "正在导入中...";
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
            JS.DownloadFile($"{Model.Name}导入模板.xlsx", stream);
        }
    }
}