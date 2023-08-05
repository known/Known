using WebSite.Docus.View.GroupBoxs;

namespace WebSite.Docus.View;

class KGroupBox : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 显示分组信息
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<GroupBox1>("1.默认示例");
    }
}