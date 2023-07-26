namespace WebSite.Docus.Feedback.Banners;

class Banner1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Banner(StyleType.Default, "这里是默认类横幅通知！");
        builder.Banner(StyleType.Primary, "这里是主要类横幅通知！");
        builder.Banner(StyleType.Success, "这里是成功类横幅通知！");
        builder.Banner(StyleType.Info, "这里是信息类横幅通知！");
        builder.Banner(StyleType.Warning, "这里是警告类横幅通知！");
        builder.Banner(StyleType.Danger, "这里是危险类横幅通知！");
    }
}