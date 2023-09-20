namespace Known.Razor.Components;

class Importer : BaseComponent
{
    private Form form;
    private bool isFinished;
    private string fileInfo;
    private string error;
    private string message;
    private readonly List<Dictionary<string, string>> datas = new();

    private List<string> Columns
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(Option.Template))
                return Option.Template.Split(',').ToList();

            return Option.Model.GetImportColumns();
        }
    }

    [Parameter] public ImportOption Option { get; set; }

    protected override Task OnInitializedAsync()
    {
        isFinished = Option.Model.IsFinished;
        error = Option.Model.Error;
        message = Option.Model.Message;
        return base.OnInitializedAsync();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<Form>()
               .Set(c => c.Style, "import-form")
               .Set(c => c.Model, Option.Model)
               .Set(c => c.ChildContent, BuildFields)
               .Build(value => form = value);
    }

    private void BuildFields(RenderTreeBuilder builder)
    {
        builder.Hidden(nameof(ImportFormInfo.Type));
        builder.Hidden(nameof(ImportFormInfo.BizId));
        builder.Hidden(nameof(ImportFormInfo.BizName));
        builder.Hidden(nameof(ImportFormInfo.BizType));
        builder.Hidden(nameof(ImportFormInfo.IsThumb));
        builder.Div("danger", "提示: 请上传单个txt或Excel格式附件！");
        builder.Div("form-item", attr =>
        {
            builder.Field<Upload>("Upload").Required(true).Enabled(isFinished)
                   .Set(f => f.Accept, "text/plain,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                   .Set(f => f.OnFilesChanged, OnFilesChanged)
                   .Build();
            builder.Button("导入", "fa fa-check", Callback(OnImport), StyleType.Primary, isFinished);
        });
        builder.Div(attr =>
        {
            builder.Link("模板下载", Callback(OnDownloadTemplate));
            if (!string.IsNullOrWhiteSpace(error))
                builder.Link("错误信息", Callback(e => UI.Alert(error, true)));
            builder.Span("size", fileInfo);
        });
        var style = string.IsNullOrWhiteSpace(error) ? "primary" : "danger";
        builder.Div($"message {style}", message);
    }

    private async void OnFilesChanged(List<IBrowserFile> files)
    {
        var file = files.FirstOrDefault();
        if (Option.Action != null)
        {
            datas.Clear();
            var text = string.Empty;
            var stream = file.OpenReadStream(Upload.MaxLength);
            if (file.Name.EndsWith(".txt"))
            {
                text = await new StreamReader(stream).ReadToEndAsync();
            }
            else
            {
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                ms.Position = 0;
                text = await UI.ExcelImport(ms);
            }

            if (!string.IsNullOrWhiteSpace(text))
            {
                var lines = text.Split(Environment.NewLine.ToArray());
                var columns = Columns;
                foreach (var item in lines)
                {
                    if (string.IsNullOrWhiteSpace(item))
                        continue;

                    var items = item.Split('\t');
                    if (items[0] == columns[0])
                        continue;

                    var data = new Dictionary<string, string>();
                    for (int i = 0; i < columns.Count; i++)
                    {
                        data[columns[i]] = items.Length > i ? items[i] : "";
                    }
                    datas.Add(data);
                }
            }
        }
        fileInfo = $"大小:{file.Size / 1024}KB";
        StateChanged();
    }

    private async void OnImport()
    {
        if (!form.Validate())
            return;

        message = "正在导入中...";
        EnableControl(false);

        if (Option.Action != null)
        {
            var result = await Option.Action?.Invoke(datas);
            if (!result.IsValid)
            {
                message = result.Message;
                EnableControl(true);
                return;
            }
            UI.Toast(result.Message);
            UI.CloseDialog();
        }
        else
        {
            form.SubmitFilesAsync(Platform.File.UploadFilesAsync, result =>
            {
                if (!result.IsValid)
                {
                    message = result.Message;
                    EnableControl(true);
                    return;
                }

                if (Option.Model.IsAsync)
                {
                    message = result.Message;
                    EnableControl(false);
                }
                else
                {
                    Option.OnSuccess?.Invoke();
                    UI.CloseDialog();
                }
            });
        }
    }

    private void EnableControl(bool isImport)
    {
        isFinished = isImport;
        StateChanged();
    }

    private async void OnDownloadTemplate()
    {
        var bytes = await Platform.File.GetImportRuleAsync(Option.Id);
        if (bytes != null && bytes.Length > 0)
        {
            var stream = new MemoryStream(bytes);
            UI.DownloadFile($"{Option.Name}导入模板.xlsx", stream);
            return;
        }

        var columns = Columns;
        if (columns == null || columns.Count == 0)
        {
            UI.Alert("导入模板不存在！");
            return;
        }

        var datas = new List<string[]> { columns.ToArray() };
        UI.ExcelExport($"{Option.Name}导入模板.xlsx", datas);
    }
}