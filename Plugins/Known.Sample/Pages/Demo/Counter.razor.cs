namespace Known.Sample.Pages.Demo;

public partial class Counter
{
    private int currentCount = 0;
    private string field;
    private string[] value = [];
    private FormModel<TestInfo> model;
    private TestInfo test = new();
    private KUpload upload;
    private int fileCount;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        model = new FormModel<TestInfo>(this)
        {
            Data = new TestInfo()
        };
    }

    private void IncrementCount()
    {
        currentCount++;
    }

    private void OnForm()
    {
        var form = new FormModel<TestInfo>(this, true)
        {
            Title = "自动表单",
            Info = new FormInfo { Width = 600, SmallLabel = true },
            //IsView = true,
            Data = new TestInfo
            {
                Field = "test",
                Organize = "研发",
                UserName = "lily",
                Date = DateTime.Now,
                Text = "测试文本"
            },
            OnSave = d =>
            {
                UI.Alert($"保存成功！文本内容：\r\n{Utils.ToJson(d)}");
                return Result.SuccessAsync("保存成功！");
            }
        };
        UI.ShowForm(form);
    }

    private void OnLoad()
    {
        App?.ShowSpinAsync("耗时操作中...", () =>
        {
            Thread.Sleep(10000);
            return Task.CompletedTask;
        });
    }

    private void OnLog()
    {
        var model = new DialogModel
        {
            Title = "控制台日志",
            Mask = false,
            Width = 600,
            Maximizable = true,
            Content = b => b.Component<KConsole>()
                            .Set(c => c.BizId, "Test")
                            .Set(c => c.MethodName, AppConstant.AddLog)
                            .Build()
        };
        UI.ShowDialog(model);
    }

    private async Task OnPaste()
    {
        await JSRuntime.PasteTextAsync(text => UI.Alert(text));
    }

    private void OnAI()
    {
        var info = new AgentInfo
        {
            Id = "test",
            Name = "测试",
            EnableFile = true,
            Model = new ModelInfo { Type = ChatType.Mock }
        };
        UI.ShowAIDrawer(info);
    }

    private Task OnFilesChanged(string id, List<FileDataInfo> files)
    {
        fileCount = files.Count;
        upload.Clear();
        return Task.CompletedTask;
    }

    private KScanner scanner;
    private string scanResult;
    private bool IsScanning => scanner?.IsScanning == true;
    private string ScanName => IsScanning ? "停止扫码" : "开始扫码";

    private async Task OnStartScan()
    {
        if (scanner.IsScanning)
            await scanner.StopAsync();
        else
            await scanner.StartAsync();
    }

    private void OnScan()
    {
        UI.ShowScanner(OnScanned);
    }

    private Task OnScanned(string text, string error)
    {
        scanResult = text;
        if (!string.IsNullOrWhiteSpace(error))
            UI.Error(error);
        return StateChangedAsync();
    }

    private List<CodeInfo> Items1 = new()
    {
        new CodeInfo {
            Code = "A", Name = "选项A", Data = new List<CodeInfo> {
                new CodeInfo { Code = "A1", Name = "选项A1" },
                new CodeInfo { Code = "A2", Name = "选项A2" }
            }
        },
        new CodeInfo {
            Code = "B", Name = "选项B", Data = new List<CodeInfo> {
                new CodeInfo { Code = "B1", Name = "选项B1" },
                new CodeInfo { Code = "B", Name = "选项B2" }
            }
        },
        new CodeInfo {
            Code = "C", Name = "选项C", Data = new List<CodeInfo> {
                new CodeInfo { Code = "C1", Name = "选项C1" },
                new CodeInfo { Code = "C2", Name = "选项C2" }
            }
        }
    };
    private List<CodeInfo> Items2 = [];

    private void OnItem1Changed(CodeInfo item)
    {
        Items2 = item.DataAs<List<CodeInfo>>();
    }
}