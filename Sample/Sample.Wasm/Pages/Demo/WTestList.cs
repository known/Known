namespace Sample.Pages.Demo;

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
public class WTestList : BizTablePage<WeatherForecast>
{
    protected override Task OnParameterAsync()
    {
        Table.Name = "天气测试";
        return base.OnParameterAsync();
    }

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Table = new TableModel<WeatherForecast>(this);
        Table.ShowPager = true;
        Table.OnQuery = OnQueryWeatherForecastsAsync;

        //Table.AddColumn(c => c.Date, true);
        //Table.AddColumn(c => c.TemperatureC);
        //Table.AddColumn(c => c.Summary);
        //Table.AddColumn(c => c.Info.Summary1).Template((b, r) => b.Text(r.Info.Summary1));

        Table.QueryActions.Add(nameof(New));
        Table.QueryActions.Add(nameof(Export));

        Table.ExpandTemplate = (b, r) => b.Text(r.Summary);
        Table.ActionCount = 4;

        Table.Tab.AddTab("北京");
        Table.Tab.AddTab("上海");
        Table.Tab.OnChange = async t =>
        {
            UI.Info(t);
            await RefreshAsync();
        };
    }

    [Action] public void New() => Table.NewForm(SaveDataAsync, new WeatherForecast());
    [Action] public void Edit(WeatherForecast row) => Table.EditForm(SaveDataAsync, row);
    [Action] public void Delete(WeatherForecast row) => Table.Delete(SaveDataAsync, row);
    [Action(Group = "Test", Name = "测试1")] public void Test1(WeatherForecast row) => UI.Alert($"{row.Summary}-Test1");
    [Action(Group = "Test", Name = "测试2")] public void Test2(WeatherForecast row) => UI.Alert($"{row.Summary}-Test2");
    [Action] public void MoveUp(WeatherForecast row) => Table.Delete(SaveDataAsync, row);
    [Action] public void MoveDown(WeatherForecast row) => Table.Delete(SaveDataAsync, row);

    public void Export() => UI.Alert("测试查询导出！");

    private async Task<PagingResult<WeatherForecast>> OnQueryWeatherForecastsAsync(PagingCriteria criteria)
    {
        await Task.Delay(500);

        var startDate = DateTime.Now;
        var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
        var forecasts = Enumerable.Range(1, criteria.PageSize).Select(index => new WeatherForecast
        {
            Date = startDate.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = summaries[Random.Shared.Next(summaries.Length)],
            Info = new WeatherInfo { Date1 = startDate, Summary1 = "Item.Summary" }
        }).ToList();
        return new PagingResult<WeatherForecast>(100, forecasts);
    }

    private Task<Result> SaveDataAsync(WeatherForecast row)
    {
        return Result.SuccessAsync("保存成功！");
    }
}