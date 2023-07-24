using WebSite.Docus.Feedback.Notifys;

namespace WebSite.Docus.Feedback;

class KNotify : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildList(new string[]
        {
            "通知提醒默认位于右下角",
            "支持默认、主要、成功、信息、警告、危险样式",
            "默认5000毫秒后自动关闭，可自定义",
            "内容支持html字符"
        });
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Notify1>();
        builder.BuildDemo<Notify2>();
    }
}