using WebSite.Docus.Nav.Tabses;

namespace WebSite.Docus.Nav;

class KTabs : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildList(new string[]
        {
            "选项卡组件",
            "支持左右上下4种布局，默认顶部"
        });
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Tabs1>("1.顶部示例");
        builder.BuildDemo<Tabs2>("2.底部示例");
        builder.BuildDemo<Tabs3>("3.左侧示例");
        builder.BuildDemo<Tabs4>("4.右侧示例");
    }
}