using WebSite.Docus.Inputs.Inputs;

namespace WebSite.Docus.Inputs;

class DInput : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 基于HTML5标签实现
- 支持Color, Email, Range, Search, Tel, Url类型
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Input1>("1.Color示例");
        builder.BuildDemo<Input2>("2.Email示例");
        builder.BuildDemo<Input3>("3.Range示例");
        builder.BuildDemo<Input4>("4.Search示例");
        builder.BuildDemo<Input5>("5.Tel示例");
        builder.BuildDemo<Input6>("6.Url示例");
    }
}