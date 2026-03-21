using Known.Sample.Reports;
using Known.Sample.Pages.Demo.Forms;
using Microsoft.AspNetCore.Components;

namespace Known.Server.Pages;

[Route("/web/tabform")]
[Menu(AppConstant.Demo, "Web标签", "form", 5)]
public class TestTabPage : BaseTabPage
{
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();

        Tab.AddTab<BasicSetting>("基本设置");
        Tab.AddTab<TestSettingList>("列表设置");
    }
}

[TabRole(typeof(TestTabPage), "列表设置")]
public class TestSettingList : BaseTable<Weather_Forecast>
{
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();

        Table.ShowName = false;
        Table.AutoHeight = true;
        Table.OnQuery = TestData.QueryWeathersAsync;
        Table.FormType = typeof(WeatherForm);
    }

    [Action] public void New() => Table.NewForm(TestData.SaveWeatherAsync, new Weather_Forecast());
    [Action] public void DeleteM() { }
}

[Route("/rpt/test")]
[Menu(AppConstant.Demo, "报表中心", "line-chart", 7)]
public class TestReport : ReportList
{
    protected override string BizType => "测试模块";
}