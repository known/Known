using WebSite.Docus.Nav.Pagers;

namespace WebSite.Docus.Nav;

class DPager : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 分页组件自适应PC和移动端
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Pager1>("1.默认示例", "block");
        builder.BuildDemo<Pager2>("2.更新总条数示例", "block");
    }
}