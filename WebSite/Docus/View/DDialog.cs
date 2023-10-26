using WebSite.Docus.View.Dialogs;

namespace WebSite.Docus.View;

class DDialog : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 弹窗显示内容
- 弹出Alert、Confirm提示框
- 弹出敏捷表单内容
- 弹窗自定义组件内容
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Dialog1>("1.默认示例");
        builder.BuildDemo<Dialog2>("2.敏捷表单示例");
        builder.BuildDemo<Dialog3>("3.组件示例");
    }
}