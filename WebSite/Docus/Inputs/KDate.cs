using WebSite.Docus.Inputs.Dates;

namespace WebSite.Docus.Inputs;

class KDate : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 基于HTML5标签实现
- 支持Date, DateTime, Month, Week, Time类型
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Date1>("1.Date示例");
        builder.BuildDemo<Date2>("2.DateTime示例");
        builder.BuildDemo<Date3>("3.Month示例");
        builder.BuildDemo<Date4>("4.Week示例");
        builder.BuildDemo<Date5>("5.Time示例");
    }
}