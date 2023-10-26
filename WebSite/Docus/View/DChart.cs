using WebSite.Docus.View.Charts;

namespace WebSite.Docus.View;

class DChart : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 基于Highcharts.js实现
- 参考网站：[Highcharts.js](https://www.hcharts.cn/)
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Chart1>("1.饼图示例");
        builder.BuildDemo<Chart2>("2.柱状图示例");
        builder.BuildDemo<Chart3>("3.线性图示例");
    }
}