namespace WebSite.Docus.Feedback.Toasts;

class Toast2 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Button("自定义内容", Callback(OnContent), StyleType.Primary);
    }

    private void OnContent() => UI.Toast("<h1>这里是自定义内容提示！</h1>", StyleType.Primary);
}