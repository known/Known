namespace WebSite.Docus.Feedback.Banners;

[Title("默认示例")]
class Banner1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Banner(StyleType.Default, "这里是默认横幅通知！");
        builder.Banner(StyleType.Primary, "这里是主要横幅通知！");
        builder.Banner(StyleType.Success, "这里是成功横幅通知！");
        builder.Banner(StyleType.Info, "这里是信息横幅通知！");
        builder.Banner(StyleType.Warning, "这里是警告横幅通知！");
        builder.Banner(StyleType.Danger, "这里是危险横幅通知！");
    }
}