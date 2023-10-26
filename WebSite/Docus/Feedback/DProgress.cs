using WebSite.Docus.Feedback.Progress;

namespace WebSite.Docus.Feedback;

class DProgress : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 支持默认、主要、成功、信息、警告、危险样式
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Progress1>("1.默认示例");
        builder.BuildDemo<Progress2>("2.固定宽度示例");
    }
}