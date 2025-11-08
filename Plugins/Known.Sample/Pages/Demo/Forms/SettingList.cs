namespace Known.Sample.Pages.Demo.Forms;

[TabRole(typeof(WTabPage), "列表设置")]
public class SettingList : BaseTable<WeatherForecast>
{
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();

        Table.ShowName = false;
        Table.AutoHeight = true;
        Table.OnQuery = TestData.QueryWeathersAsync;
        Table.FormType = typeof(WeatherForm);
    }

    [Action] public void New() => Table.NewForm(TestData.SaveWeatherAsync, new WeatherForecast());
}