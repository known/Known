namespace Sample.Razor.Samples;

class DemoChart : BaseComponent
{
    private readonly ChartDataInfo[] datas1;
    private readonly ChartDataInfo[] datas2;
    private Chart chart1;
    private Chart chart2;
    private Chart chart3;

    public DemoChart()
    {
        datas1 = new ChartDataInfo[] {
            new ChartDataInfo
            {
                Name="分类",
                Series=new Dictionary<string, object>{
                    {"分类一",50},{"分类二",30},{"分类三",15},{"分类四",5}
                }
            }
        };

        var data1 = new ChartDataInfo { Name = "类型一", Series = new Dictionary<string, object>() };
        var data2 = new ChartDataInfo { Name = "类型二", Series = new Dictionary<string, object>() };
        for (int i = 2010; i < DateTime.Now.Year; i++)
        {
            var rand1 = new Random();
            data1.Series[$"{i}"] = rand1.Next(100, 1000);
            var rand2 = new Random();
            data2.Series[$"{i}"] = rand2.Next(200, 1000);
        }
        datas2 = new ChartDataInfo[] { data1, data2 };
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("row", attr =>
        {
            builder.Component<Chart>()
                   .Set(c => c.Id, "chart1")
                   .Build(value => chart1 = value);
            builder.Component<Chart>()
                   .Set(c => c.Id, "chart2")
                   .Build(value => chart2 = value);
            builder.Component<Chart>()
                   .Set(c => c.Id, "chart3")
                   .Build(value => chart3 = value);
        });
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            chart1.YAxis = new { title = new { text = "数量" } };
            chart1.ShowPie("类型统计", datas1);

            chart2.YAxis = new { title = new { text = "数量" } };
            chart2.ShowBar("柱状图统计", datas2);

            chart3.YAxis = new { title = new { text = "数量" } };
            chart3.ShowLine("折线图统计", datas2);
        }
    }
}