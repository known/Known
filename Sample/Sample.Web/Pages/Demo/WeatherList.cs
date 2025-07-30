using Microsoft.AspNetCore.Components.Web;

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
        Table = new TableModel<WeatherForecast>(this, TableColumnMode.Attribute);
        Table.ShowPager = true;
        Table.OnQuery = OnQueryWeatherForecastsAsync;
        Table.FormType = typeof(WeatherForm);

        Table.AddColumn(c => c.Info.Summary1).Template((b, r) => b.Text(r.Info.Summary1));

        Table.Tab.AddTab("北京");
        Table.Tab.AddTab("上海");
        Table.Tab.OnChange = async t =>
        {
            UI.Info(t);
            await RefreshAsync();
        };
    }

    [Action] public void New() => Table.NewForm(SaveDataAsync, new WeatherForecast());

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

class WeatherForm : BaseForm<WeatherForecast>
{
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Model.FooterLeft = b => b.CheckBox(new InputModel<bool> { Label = "是否关闭" });
        Model.AddAction("测试", this.Callback<MouseEventArgs>(e => UI.Alert("Test OK!")));
    }
}

public class WeatherForecast
{
    [Column(IsQuery = true)]
    [Form(Type = nameof(FieldType.Date))]
    [DisplayName("日期")]
    public DateTime? Date { get; set; }

    [Column(Width = 150)]
    [Form]
    public int TemperatureC { get; set; }

    [Column(Type = FieldType.File)]
    [Form]
    public string Summary { get; set; }

    [Column(Width = 150)]
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public WeatherInfo Info { get; set; }
}

public class WeatherInfo
{
    public DateTime? Date1 { get; set; }
    public string Summary1 { get; set; }
}