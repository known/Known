using WebSite.Docus.View.Dialogs;

namespace WebSite.Docus.View;

class KDialog : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildList(new string[]
        {
            "弹窗显示内容",
            "弹出Alert、Confirm提示框",
            "弹窗自定义组件内容",
        });
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Dialog1>("1.默认示例");
        builder.BuildDemo<Dialog2>("2.组件示例");
    }
}