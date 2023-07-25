using WebSite.Docus.Feedback.Progress;

namespace WebSite.Docus.Feedback;

class KProgress : BaseDocu
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
        builder.BuildDemo<Progress1>();
        builder.BuildDemo<Progress2>();
    }
}