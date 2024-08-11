using Known.AntBlazor.Components;

namespace Sample.Client.Pages;

partial class TestForm
{
    private List<CodeInfo> Texts = [];

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Model = new FormModel<TestInfo>(this) { Data = new TestInfo() };
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            Texts.Clear();
            for (int i = 0; i < 500; i++)
            {
                Texts.Add(new CodeInfo($"Code{i}", $"测试名称{i}"));
            }
        }
    }
}

public class TestInfo
{
    public string Text { get; set; }
    public int? Number { get; set; }
    public decimal? Decimal { get; set; }
    public DateTime? Date { get; set; }
    public string Note { get; set; }
}

public class TestInfoForm : AntForm<TestInfo> { }