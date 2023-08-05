using WebSite.Docus.Nav.Trees;

namespace WebSite.Docus.Nav;

class KTree : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 树组件
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Tree1>("1.默认示例");
        builder.BuildDemo<Tree2>("2.操作示例");
    }
}