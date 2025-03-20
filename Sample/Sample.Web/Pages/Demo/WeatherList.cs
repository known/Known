namespace Sample.Web.Pages.Demo;

[Route("/weathers/{id?}")]
public class WeatherList : BaseTablePage<WeatherForecast>
{
    protected override Task OnParameterAsync()
    {
        Table.Name = $"天气列表 - {Id}";
        return base.OnParameterAsync();
    }

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Table = new TableModel<WeatherForecast>(this);
        Table.ShowPager = true;
        Table.OnQuery = OnQueryWeatherForecastsAsync;

        Table.AddColumn(c => c.Date, true);
        Table.AddColumn(c => c.TemperatureC);
        Table.AddColumn(c => c.Summary);
        Table.AddColumn(c => c.Info.Summary1).Template((b, r) => b.Text(r.Info.Summary1));

        Table.Tab.AddTab("北京");
        Table.Tab.AddTab("上海");
        Table.Tab.OnChange = async t =>
        {
            UI.Info(t);
            await RefreshAsync();
        };
    }

    private async Task<PagingResult<WeatherForecast>> OnQueryWeatherForecastsAsync(PagingCriteria criteria)
    {
        await Task.Delay(500);

        var startDate = DateOnly.FromDateTime(DateTime.Now);
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
}

public class WeatherForecast
{
    [Column(IsQuery = true)]
    [DisplayName("日期")]
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public string Summary { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public WeatherInfo Info { get; set; }
}

public class WeatherInfo
{
    public DateOnly Date1 { get; set; }
    public string Summary1 { get; set; }
}