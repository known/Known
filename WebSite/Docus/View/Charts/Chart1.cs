namespace WebSite.Docus.View.Charts;

class Chart1 : BaseComponent
{
    private KChart chart;
    private readonly ChartDataInfo[] datas = new ChartDataInfo[] {
        new ChartDataInfo
        {
            Name = "分类",
            Series = new Dictionary<string, object>{
                {"分类一",50},{"分类二",30},{"分类三",15},{"分类四",5}
            }
        }
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<KChart>().Id("chart1").Build(value => chart = value);
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            chart.YAxis = new { title = new { text = "数量" } };
            chart.ShowPie("类型统计", datas);
        }
    }
}