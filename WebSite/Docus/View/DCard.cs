using WebSite.Docus.View.Cards;

namespace WebSite.Docus.View;

class DCard : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 由标题和内容组成
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Card1>("1.默认示例");
        builder.BuildDemo<Card2>("2.Head模板示例");
    }
}