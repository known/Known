namespace Sample.Web.Tests.Pages;

[Route("/weathers")]
public class WeatherList : BaseTablePage<WeatherForecast>
{
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Table = new TableModel<WeatherForecast>(this, TableColumnMode.Property);
        Table.ShowPager = true;
        Table.Name = "天气";
        Table.OnQuery = OnQueryWeatherForecastsAsync;
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
            Summary = summaries[Random.Shared.Next(summaries.Length)]
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
}