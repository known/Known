namespace WebSite.Docus.Input.Buttons;

[Title("文本按钮示例")]
class Button1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Button("默认", Callback(OnClick), StyleType.Default);
        builder.Button("主要", Callback(OnClick), StyleType.Primary);
        builder.Button("成功", Callback(OnClick), StyleType.Success);
        builder.Button("信息", Callback(OnClick), StyleType.Info);
        builder.Button("警告", Callback(OnClick), StyleType.Warning);
        builder.Button("危险", Callback(OnClick), StyleType.Danger);
    }

    private void OnClick() => UI.Toast("点击测试！");
}