using WebSite.Docus.View.Timelines;

namespace WebSite.Docus.View;

class KTimeline : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildList(new string[]
        {
            "支持自定义模板"
        });
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Timeline1>("1.默认示例");
        builder.BuildDemo<Timeline2>("2.状态示例");
        builder.BuildDemo<Timeline3>("3.自定义模板示例");
    }
}