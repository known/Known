namespace WebSite.Docus.Input.Buttons;

[Title("禁用按钮示例")]
class Button3 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Button("默认", Callback(OnClick), StyleType.Default, false);
        builder.Button("主要", Callback(OnClick), StyleType.Primary, false);
        builder.Button("成功", Callback(OnClick), StyleType.Success, false);
        builder.Button("信息", "fa fa-info-circle", Callback(OnClick), StyleType.Info, false);
        builder.Button("警告", "fa fa-exclamation-circle", Callback(OnClick), StyleType.Warning, false);
        builder.Button("危险", "fa fa-times-circle-o", Callback(OnClick), StyleType.Danger, false);
    }

    private void OnClick() => UI.Toast("点击测试！");
}