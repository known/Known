namespace Known.Sample.Pages.Demo;

public partial class Weather
{
    //private TableModel<WeatherForecast> model;
    private TableModel<Weather_Forecast> model1;
    private TableModel<Weather_Forecast> model2;

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        // model = new TableModel<WeatherForecast>(this, TableColumnMode.Property);
        // model.ShowPager = true;
        // model.Name = "天气";
        // model.OnQuery = OnQueryWeatherForecastsAsync;
        // model.Toolbar.AddAction(nameof(New));

        model1 = new TableModel<Weather_Forecast>(this, "Weather1", TableColumnMode.Property);
        model1.ShowPager = true;
        model1.EnableSpin = true;
        model1.Name = "天气1";
        model1.OnQuery = TestData.QueryWeathersAsync;
        model1.Toolbar.AddAction(nameof(New));
        model1.Toolbar.AddAction(nameof(Test1), "", "user", "");
        model1.AddIconAction(nameof(Test2), "user");

        model2 = new TableModel<Weather_Forecast>(this, "Weather2", TableColumnMode.Property);
        model2.ShowPager = true;
        model2.Name = "天气2";
        model2.OnQuery = TestData.QueryWeathersAsync;
    }

    public void New()
    {
        // model1.NewForm(d =>
        // {
        //     UI.Alert($"Summary={d.Summary}");
        //     return Known.Result.SuccessAsync("");
        // });
        var model = new DialogModel
        {
            Title = "天气测试",
            Width = 1000,
            Content = b => b.Component<TestRole>().Build()
        };
        UI.ShowDialog(model);
    }

    public void Test1() => UI.Alert("Test");
    public void Test2(Weather_Forecast row) => UI.Alert(row.Summary);
}