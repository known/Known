using WebSite.Docus.Feedback.Banners;

namespace WebSite.Docus.Feedback;

class KBanner : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildList(new string[]
        {
            "横幅可嵌入任何位置",
            "支持默认、主要、成功、信息、警告、危险样式",
            "默认手动点击图标关闭"
        });
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Banner1>();
        builder.BuildDemo<Banner2>();
    }
}