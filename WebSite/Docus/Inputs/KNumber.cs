using WebSite.Docus.Inputs.Numbers;

namespace WebSite.Docus.Inputs;

class KNumber : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 基于HTML5标签实现
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Number1>("1.默认示例");
        builder.BuildDemo<Number2>("2.控制示例");
        builder.BuildDemo<Number3>("3.QPA联动示例");
    }
}