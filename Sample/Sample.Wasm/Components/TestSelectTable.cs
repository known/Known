namespace Sample.Components;

public class TestSelectTable : AntDropdownTable<WeatherForecast>
{
    protected override Func<WeatherForecast, string> OnValue => d => d.Summary;

    protected override async Task OnInitializeAsync()
    {
        await base.OnInitializeAsync();

        Table.Form = new FormInfo { Width = 800 };
        //Table.FormType = typeof(AccountForm);
        Table.OnQuery = TestData.QueryWeathersAsync;
        Table.AddColumn(c => c.Summary, true);
        Table.AddColumn(c => c.Date);
        Table.AddColumn(c => c.TemperatureC);
        Table.AddColumn(c => c.TemperatureF);

        Table.Toolbar.AddAction(nameof(New));
    }

    public void New() => Table.NewForm(TestData.SaveWeatherAsync, new WeatherForecast());
}