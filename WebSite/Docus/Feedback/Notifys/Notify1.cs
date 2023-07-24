namespace WebSite.Docus.Feedback.Notifys;

[Title("默认示例")]
class Notify1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Button("默认", Callback(OnDefault));
        builder.Button("主要", Callback(OnPrimary), StyleType.Primary);
        builder.Button("成功", Callback(OnSuccess), StyleType.Success);
        builder.Button("信息", Callback(OnInfo), StyleType.Info);
        builder.Button("警告", Callback(OnWarning), StyleType.Warning);
        builder.Button("危险", Callback(OnDanger), StyleType.Danger);
    }

    private void OnDefault() => UI.Notify("这里是默认类通知！", StyleType.Default);
    private void OnPrimary() => UI.Notify("这里是主要类通知！", StyleType.Primary);
    private void OnSuccess() => UI.Notify("这里是成功类通知！", StyleType.Success);
    private void OnInfo() => UI.Notify("这里是信息类通知！", StyleType.Info);
    private void OnWarning() => UI.Notify("这里是警告类通知！", StyleType.Warning);
    private void OnDanger() => UI.Notify("这里是危险类通知！", StyleType.Danger);
}