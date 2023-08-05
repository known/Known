using WebSite.Docus.Nav.Stepses;

namespace WebSite.Docus.Nav;

class KSteps : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 分步表单组件
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Steps1>("1.默认示例");
    }
}