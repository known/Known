namespace WebSite.Docus.Feedback.Notifys;

[Title("自定义示例")]
class Notify2 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Button("自定义内容", Callback(OnContent), StyleType.Primary);
        builder.Button("自定义关闭时间", Callback(OnTime), StyleType.Primary);
    }

    private void OnContent() => UI.Notify("<h1>这里是自定义内容通知！</h1>", StyleType.Primary);
    private void OnTime() => UI.Notify("这里是自定义关闭时间通知！<br/>10秒后自动关闭。", StyleType.Primary, 10000);
}