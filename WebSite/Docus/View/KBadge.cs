using WebSite.Docus.View.Badges;

namespace WebSite.Docus.View;

class KBadge : BaseDocu
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
        builder.BuildDemo<Badge1>("1.默认示例");
    }
}