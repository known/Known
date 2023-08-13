using WebSite.Docus.Feedback.Loadings;

namespace WebSite.Docus.Feedback;

class KLoading : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 用于耗时操作提示
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Loading1>("1.默认示例");
    }
}