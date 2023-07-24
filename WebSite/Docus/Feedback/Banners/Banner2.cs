namespace WebSite.Docus.Feedback.Banners;

[Title("自定义模板示例")]
class Banner2 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Banner(StyleType.Primary, b => b.Span("fa fa-check bold", "这里是自定义横幅通知！"));
    }
}