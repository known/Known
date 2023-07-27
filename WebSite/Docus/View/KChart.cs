using WebSite.Docus.View.Charts;

namespace WebSite.Docus.View;

class KChart : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildList(new string[]
        {
            "基于Highcharts.js实现"
        });
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Chart1>("1.饼图示例");
        builder.BuildDemo<Chart2>("2.柱状图示例");
        builder.BuildDemo<Chart3>("3.线性图示例");
    }
}