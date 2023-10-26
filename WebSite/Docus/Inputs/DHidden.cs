using WebSite.Docus.Inputs.Hiddens;

namespace WebSite.Docus.Inputs;

class DHidden : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 隐藏字段组件
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Hidden1>("1.默认示例");
    }
}