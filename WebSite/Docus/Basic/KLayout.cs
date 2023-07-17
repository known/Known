using Known.Razor.Components;

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
        builder.BuildDemo<KLayout1>("layout", "默认布局", "class KLayout1 : Layout { }");
        builder.BuildDemo<KLayout2>("layout", "HS布局", @"class KLayout2 : Layout
{
    public KLayout2()
    {
        Style = ""layout-tl"";
    }
}");
        builder.BuildDemo<KLayout3>("layout", "自定义布局", @"class KLayout3 : Layout
{
    protected override void BuildHeader(RenderTreeBuilder builder)
    {
        builder.Span(""自定义Header"");
    }

    protected override void BuildSider(RenderTreeBuilder builder)
    {
        builder.Span(""自定义Sider"");
    }

    protected override void BuildBody(RenderTreeBuilder builder)
    {
        builder.Span(""自定义Body"");
    }
}");
    }

    class KLayout1 : Layout { }

    class KLayout2 : Layout
    {
        public KLayout2()
        {
            Style = "layout-tl";
        }
    }

    class KLayout3 : Layout
    {
        protected override void BuildHeader(RenderTreeBuilder builder)
        {
            builder.Span("自定义Header");
        }

        protected override void BuildSider(RenderTreeBuilder builder)
        {
            builder.Span("自定义Sider");
        }

        protected override void BuildBody(RenderTreeBuilder builder)
        {
            builder.Span("自定义Body");
        }
    }
}