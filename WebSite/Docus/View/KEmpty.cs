using WebSite.Docus.View.Emptys;

namespace WebSite.Docus.View;

class KEmpty : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildList(new string[]
        {
            "用于无数据提示"
        });
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Empty1>("1.默认示例");
    }
}