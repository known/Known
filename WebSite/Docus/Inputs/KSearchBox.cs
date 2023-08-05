using WebSite.Docus.Inputs.SearchBoxs;

namespace WebSite.Docus.Inputs;

class KSearchBox : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 搜索框组件
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<SearchBox1>("1.默认示例");
    }
}