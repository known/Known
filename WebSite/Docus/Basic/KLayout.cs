using WebSite.Docus.Basic.Layouts;

namespace WebSite.Docus.Basic;

class KLayout : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildList(new string[]
        {
            "内置后台主页默认布局为Header、Sider、Body组合",
            "布局Header和Sider颜色可通过主题设置自定义",
            "Body支持单页和多标签页",
            "可自定义后台主页布局，重写Index页面的BuildAdmin方法"
        });
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<KLayout1>("layout");
        builder.BuildDemo<KLayout2>("layout");
        builder.BuildDemo<KLayout3>("layout");
    }
}