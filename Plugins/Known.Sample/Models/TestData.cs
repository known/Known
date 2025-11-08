namespace Known.Sample.Models;

class TestData
{
    internal static async Task<PagingResult<WeatherForecast>> QueryWeathersAsync(PagingCriteria criteria)
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

    internal static Task<Result> SaveWeatherAsync(WeatherForecast row)
    {
        return Result.SuccessAsync("保存成功！");
    }
}