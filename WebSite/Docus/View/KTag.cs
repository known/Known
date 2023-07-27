using WebSite.Docus.View.Tags;

namespace WebSite.Docus.View;

class KTag : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildList(new string[]
        {
            "支持默认、主要、成功、信息、警告、危险样式"
        });
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Tag1>("1.默认示例");
    }
}