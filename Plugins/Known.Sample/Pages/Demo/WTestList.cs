namespace Known.Sample.Pages.Demo;

public class BizTablePage<TItem> : BaseTablePage<TItem> where TItem : class, new()
{
    [Action] public Task Import() => Table.ShowImportAsync();
    //[Action] public Task Export() => Table.ExportDataAsync();
    [Action(Group = "Export")] public Task ExportSelect() => Table.ExportDataAsync(ExportMode.Select);
    [Action(Group = "Export")] public Task ExportPage() => Table.ExportDataAsync(ExportMode.Page);
    [Action(Group = "Export")] public Task ExportQuery() => Table.ExportDataAsync(ExportMode.Query);
    [Action(Group = "Export")] public Task ExportAll() => Table.ExportDataAsync(ExportMode.All);
}

[Route("/wtests")]
[Menu(AppConstant.Demo, "天气测试", "cloud", 4)]
public class WTestList : BizTablePage<Weather_Forecast>
{
    protected override Task OnParameterAsync()
    {
        Table.Name = "天气测试";
        return base.OnParameterAsync();
    }

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Table = new TableModel<Weather_Forecast>(this);
        Table.ShowPager = true;
        Table.OnQuery = TestData.QueryWeathersAsync;

        //Table.AddColumn(c => c.Date, true);
        //Table.AddColumn(c => c.TemperatureC);
        //Table.AddColumn(c => c.Summary);
        //Table.AddColumn(c => c.Info.Summary1).Template((b, r) => b.Text(r.Info.Summary1));

        Table.QueryActions.Add(nameof(New));
        Table.QueryActions.Add(nameof(Export));

        Table.ExpandTemplate = (b, r) => b.Component<InnerTable>().Build();
        Table.ActionCount = 5;
        Table.ActionWidth = "200px";

        Table.Tab.AddTab("北京");
        Table.Tab.AddTab("上海");
        Table.Tab.OnChange = async t =>
        {
            UI.Info(t);
            await RefreshAsync();
        };
    }

    [Action] public void New() => Table.NewForm(TestData.SaveWeatherAsync, new Weather_Forecast());
    [Action] public void Edit(Weather_Forecast row) => Table.EditForm(TestData.SaveWeatherAsync, row);
    [Action] public void Delete(Weather_Forecast row) => Table.Delete(TestData.SaveWeatherAsync, row);

    [Action(Group = "Test", Name = "测试1")] 
    public void Test1(Weather_Forecast row) => UI.Alert($"{row.Summary}-Test1");
    
    [Action(Group = "Test", Name = "测试2")] 
    public void Test2(Weather_Forecast row) => UI.Alert($"{row.Summary}-Test2");

    [Action(Name = "测试3", Title = "测试工具栏按钮提示")]
    public void Test3() => UI.Alert("工具栏按钮测试!");

    [Action(Name = "测试3", Title = "测试操作列按钮提示")]
    public void Test3(Weather_Forecast row) => UI.Alert($"测试{row.Summary}");

    [Action] public void MoveUp(Weather_Forecast row) => Table.Delete(TestData.SaveWeatherAsync, row);
    [Action] public void MoveDown(Weather_Forecast row) => Table.Delete(TestData.SaveWeatherAsync, row);

    public void Export() => UI.Alert("测试查询导出！");
}

class InnerTable : BaseTable<Weather_Forecast>
{
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();

        Table.FixedWidth = "800px";
        Table.FixedHeight = "200px";
        Table.AddColumn(c => c.Summary);
        Table.AddColumn(c => c.TemperatureC);
        Table.AddColumn(c => c.TemperatureF);
        Table.AddColumn(c => c.Date);
    }
}