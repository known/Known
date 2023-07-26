namespace WebSite.Docus.Input.Buttons;

class Button2 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Button("默认", "fa fa-arrow-left", Callback(OnClick), StyleType.Default);
        builder.Button("主要", "fa fa-plus-circle", Callback(OnClick), StyleType.Primary);
        builder.Button("成功", "fa fa-check-circle", Callback(OnClick), StyleType.Success);
        builder.Button("信息", "fa fa-info-circle", Callback(OnClick), StyleType.Info);
        builder.Button("警告", "fa fa-exclamation-circle", Callback(OnClick), StyleType.Warning);
        builder.Button("危险", "fa fa-times-circle-o", Callback(OnClick), StyleType.Danger);
    }

    private void OnClick() => UI.Toast("点击测试！");
}