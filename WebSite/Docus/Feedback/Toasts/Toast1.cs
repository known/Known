namespace WebSite.Docus.Feedback.Toasts;

[Title("默认示例")]
class Toast1 : BaseComponent
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

    private void OnDefault() => UI.Toast("这里是默认类提示！", StyleType.Default);
    private void OnPrimary() => UI.Toast("这里是主要类提示！", StyleType.Primary);
    private void OnSuccess() => UI.Toast("这里是成功类提示！", StyleType.Success);
    private void OnInfo() => UI.Toast("这里是信息类提示！", StyleType.Info);
    private void OnWarning() => UI.Toast("这里是警告类提示！", StyleType.Warning);
    private void OnDanger() => UI.Toast("这里是危险类提示！", StyleType.Danger);
}