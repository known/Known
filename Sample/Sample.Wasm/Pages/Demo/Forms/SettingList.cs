namespace Sample.Pages.Demo.Forms;

[TabRole(typeof(WTabPage), "列表设置")]
public class SettingList : BaseTablePage<WeatherForecast>
{
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();

        Table.ShowName = false;
        Table.ShowSetting = false;
        Table.OnQuery = TestData.QueryWeathersAsync;
        Table.FormType = typeof(WeatherForm);

        Table.AddQueryAction<SettingList>(nameof(New));
    }

    [Action] public void New() => Table.NewForm(TestData.SaveWeatherAsync, new WeatherForecast());
}