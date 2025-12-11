namespace Known.Sample.Models;

public class TestData
{
    private static readonly List<Weather_Forecast> Datas = [];

    static TestData()
    {
        var startDate = DateTime.Now;
        var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
        Datas = [.. Enumerable.Range(1, 100).Select(index => new Weather_Forecast
        {
            Date = startDate.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = summaries[Random.Shared.Next(summaries.Length)],
            Info = new WeatherInfo { Date1 = startDate, Summary1 = "Item.Summary" }
        })];
    }

    public static async Task<PagingResult<Weather_Forecast>> QueryWeathersAsync(PagingCriteria criteria)
    {
        //await Task.Delay(500);
        var items = Datas;
        var key = criteria.GetParameter<string>("Key");
        if (!string.IsNullOrWhiteSpace(key))
            items = [.. items.Where(d => d.Summary.Contains(key))];
        return items.ToPagingResult(criteria);
    }

    public static Task<Result> SaveWeatherAsync(Weather_Forecast row)
    {
        return Result.SuccessAsync("保存成功！");
    }
}