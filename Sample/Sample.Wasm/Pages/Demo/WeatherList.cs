using Microsoft.AspNetCore.Components.Web;

namespace Sample.Pages.Demo;

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
        Table.OnQuery = TestData.QueryWeathersAsync;
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

    [Action] public void New() => Table.NewForm(TestData.SaveWeatherAsync, new WeatherForecast());
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