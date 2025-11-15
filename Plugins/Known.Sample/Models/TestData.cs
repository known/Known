namespace Known.Sample.Models;

class TestData
{
    internal static async Task<PagingResult<Weather_Forecast>> QueryWeathersAsync(PagingCriteria criteria)
    {
        await Task.Delay(500);

        var startDate = DateTime.Now;
        var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
        var forecasts = Enumerable.Range(1, criteria.PageSize).Select(index => new Weather_Forecast
        {
            Date = startDate.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = summaries[Random.Shared.Next(summaries.Length)],
            Info = new WeatherInfo { Date1 = startDate, Summary1 = "Item.Summary" }
        }).ToList();
        return new PagingResult<Weather_Forecast>(100, forecasts);
    }

    internal static Task<Result> SaveWeatherAsync(Weather_Forecast row)
    {
        return Result.SuccessAsync("保存成功！");
    }
}