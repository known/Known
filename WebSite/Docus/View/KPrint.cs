using WebSite.Docus.View.Prints;

namespace WebSite.Docus.View;

class KPrint : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 打印自定义表单
- 调用浏览器打印组件
- 表单样式需写在组件中
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Print1>("1.默认示例", "block");
    }
}