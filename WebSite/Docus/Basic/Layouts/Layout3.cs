namespace WebSite.Docus.Basic.Layouts;

[Title("自定义布局")]
class Layout3 : Layout
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