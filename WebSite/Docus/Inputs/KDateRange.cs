using WebSite.Docus.Inputs.DateRanges;

namespace WebSite.Docus.Inputs;

class KDateRange : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 显示Date区间查询
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<DateRange1>("1.默认示例");
        builder.BuildDemo<DateRange2>("2.控制示例");
    }
}